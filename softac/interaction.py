from mlagents_envs.environment import UnityEnvironment
from mlagents_envs.side_channel.engine_configuration_channel import EngineConfigurationChannel
from random import  randint
from mlagents_envs.base_env import ActionTuple
import numpy as np

env = UnityEnvironment(file_name="RobotBalance.x86_64", seed=1, side_channels=[])

env.reset()

behavior_names = env.behavior_specs.keys()
print(list(behavior_names))

for i in range(10000):
    env.reset()
    for j in range(400):
        act = ActionTuple(continuous=np.array([randint(-1, 1) for k in range(20)]).reshape(-1, 20))

        env.set_actions('Spider?team=0', act)
        env.step()
        stepd = env.get_steps('Spider?team=0')
        print(stepd[0].reward)
