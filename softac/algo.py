import torch
import torch.nn as nn
import torch.nn.functional as F
import numpy as np
from buffer import Buffer
from net import actor, critic
from mlagents_envs.environment import UnityEnvironment
from mlagents_envs.side_channel.engine_configuration_channel import EngineConfigurationChannel
from random import  randint
from mlagents_envs.base_env import ActionTuple
import torch.optim as opt
import wandb


def loss(actor, critic, trajectory):
    states = torch.tensor(trajectory[0].reshape(-1, 72))

    actions = torch.tensor(trajectory[1].reshape(-1, 20))

    rewards = torch.tensor(trajectory[2].reshape(-1, 1))

    distr = actor(states)

    q_s = critic(states, actions)

    #actor_loss
    lp = distr.log_prob(actions)

    err1 = q_s+ lp

    #critic_loss


    Q_ret = torch.zeros((len(rewards), )).to(device=cuda)

    discounted_reward = torch.zeros((len(rewards), )).to(device=cuda)

    discounted_reward[-1] = rewards[-1]

    gamma = 0.99

    for i in range(len(rewards) - 2, -1, -1):
        discounted_reward[i] = gamma * discounted_reward[i + 1] + rewards[i]

    for i in range(len(rewards)):
        for j in range(i, len(rewards)):
            Q_ret[i] += 0.99 ** (j - i) * (rewards[j][0] + (discounted_reward[i] - discounted_reward[j]))

    err2 = q_s - Q_ret.reshape(-1, 1)

    return (err1 + err2).mean()



if __name__ == '__main__':

    cuda = torch.device('cuda:0')

    wandb.init(entity='dexfrost89', project='robot')
    env = UnityEnvironment(file_name="RobotBalance.x86_64", seed=1, side_channels=[], no_graphics=True)
    train_seqs = 1000
    train_eps = 2
    ep_steps = 400
    actr = actor(72)

    actr.load_state_dict(torch.load("/home/dexfrost89/saves2/actor "+str(350)))
    actr.to(device=cuda)
    crtc = critic(92)
    crtc.load_state_dict(torch.load("/home/dexfrost89/saves2/critic "+str(350)))
    crtc.to(device=cuda)

    optimizer_actor = opt.Adam(actr.parameters(), lr=0.0002)
    optimizer_critic = opt.Adam(crtc.parameters(), lr=0.0002)

    buff = Buffer()
    buff.max_size = 2
    buff.batch_size = 400

    save_time = 1

    for seq in range(351, train_seqs):
        print('Sequence', seq)
        for episode in range(train_eps):
            print('Episode', episode)
            obses, acts, rews = torch.zeros((ep_steps, 72)), torch.zeros((ep_steps, 20)), torch.zeros((ep_steps, 1))
            env.reset()

            for step in range(ep_steps):
                ns = env.get_steps('Spider?team=0')[0]
                obs = torch.tensor(ns.obs[1]).to(device=cuda)
                action = actr(obs.reshape(-1, 72)).sample()
                act_t = ActionTuple(continuous=action.to(device=torch.device('cpu')).numpy().reshape(-1, 20))
                env.set_actions('Spider?team=0', act_t)
                env.step()
                ns = env.get_steps('Spider?team=0')[0]
                reward = torch.tensor(ns.reward).to(device=cuda)
                obses[step] = obs.detach().clone()
                acts[step] = action.detach().clone()
                rews[step] = reward.detach().clone()

                obs = ns.obs[1]

            buff.add_trajectory(obses, acts, rews)

        optimizer_critic.zero_grad()
        optimizer_actor.zero_grad()
        err = 0
        for opt_step in range(2):
            print('Update', opt_step)
            traj = buff.sample_trajectory(opt_step)

            err += loss(actr, crtc, traj)

        wandb.log({'reward': np.mean(buff.rewards.to(device=torch.device('cpu')).numpy()),
                   'loss': np.mean(err.to(device=torch.device('cpu')).detach().numpy()),
                   'Step': seq})
        err.backward()

        optimizer_critic.step()
        optimizer_actor.step()

        if seq % save_time == 0:
            torch.save(actr.state_dict(), "/home/dexfrost89/saves2/actor "+str(seq))
            torch.save(crtc.state_dict(), "/home/dexfrost89/saves2/critic "+str(seq))


