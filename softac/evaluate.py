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

if __name__ == '__main__':

    cuda = torch.device('cuda:0')

    wandb.init(entity='dexfrost89', project='robot')
    env = UnityEnvironment(file_name="RobotBalance.x86_64", seed=1, side_channels=[], base_port=5004)
    actr = actor(72)
    actr.load_state_dict(torch.load("/home/dexfrost89/saves2/actor "+str(999)))
    actr.to(device=cuda)
    crtc = critic(92)
    crtc.load_state_dict(torch.load("/home/dexfrost89/saves2/critic "+str(999)))
    crtc.to(device=cuda)

    for seq in range(1):
        for episode in range(3):
            env.reset()

            for step in range(400):
                ns = env.get_steps('Spider?team=0')[0]
                obs = torch.tensor(ns.obs[1]).to(device=cuda)
                action = actr(obs.reshape(-1, 72)).mean
                act_t = ActionTuple(continuous=action.to(device=torch.device('cpu')).detach().numpy().reshape(-1, 20))
                env.set_actions('Spider?team=0', act_t)
                env.step()
                ns = env.get_steps('Spider?team=0')[0]
                reward = torch.tensor(ns.reward).to(device=cuda)


