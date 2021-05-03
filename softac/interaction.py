from mlagents_envs.environment import UnityEnvironment
from mlagents_envs.side_channel.engine_configuration_channel import EngineConfigurationChannel
from gym_unity.envs import UnityToGymWrapper
from random import  randint
from mlagents_envs.base_env import ActionTuple
import numpy as np


env = UnityToGymWrapper(UnityEnvironment(file_name="RobotBalance.x86_64", seed=1, side_channels=[], no_graphics=False), allow_multiple_obs=False)

#behavior_names = env.behavior_specs.keys()
#print(list(behavior_names))

for i in range(1):



    obs = env.reset()
    for j in range(400):
        act = np.array([randint(-1, 1) for k in range(20)], dtype=np.float).reshape(-1, 20)
        obs, reward, done, info = env.step(act)
        print(obs.shape)

    env.close()