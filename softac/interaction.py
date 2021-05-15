import sys
from os import path
from random import randint

import numpy as np

from mlagents_envs.environment import UnityEnvironment
from mlagents_envs.side_channel.engine_configuration_channel import EngineConfigurationChannel
from gym_unity.envs import UnityToGymWrapper
from mlagents_envs.base_env import ActionTuple

# For importing screen_recorder modules
sys.path.insert(1, path.join(sys.path[0], path.pardir, 'screen_recorder'))
from window_recorder import WindowRecorder

env = UnityToGymWrapper(
    UnityEnvironment(
        file_name=path.join('build', 'RobotBalance.x86_64'),
        # 'D:\\saves\\arm\\repo\\windows_build\\New Unity Project (2).exe'
        seed=1,
        side_channels=[],
        no_graphics=False),
    allow_multiple_obs=False)

#rec = WindowRecorder(window_name='Unity', log_dir_path='visual_logs/', log_name='test')
#rec.start()

for i in range(1):
    obs = env.reset()
    for j in range(100):
        act = np.array([randint(-1, 1) for k in range(5)], dtype=np.float64).reshape(-1, 5)
        obs, reward, done, info = env.step(act)
        print(i, j)
        if done:
            env.reset()

#rec.finish()

print('The end!')
