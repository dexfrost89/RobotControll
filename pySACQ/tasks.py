import random
import numpy as np


# Observation space, according to source:
# state = [
#     (pos.x - VIEWPORT_W / SCALE / 2) / (VIEWPORT_W / SCALE / 2),
#     (pos.y - (self.helipad_y + LEG_DOWN / SCALE)) / (VIEWPORT_W / SCALE / 2),
#     vel.x * (VIEWPORT_W / SCALE / 2) / FPS,
#     vel.y * (VIEWPORT_H / SCALE / 2) / FPS,
#     self.lander.angle,
#     20.0 * self.lander.angularVelocity / FPS,
#     1.0 if self.legs[0].ground_contact else 0.0,
#     1.0 if self.legs[1].ground_contact else 0.0
# ]

# Auxiliary Rewards:
# Touch. Maximizing number of legs touching ground
# Hover Planar. Minimize the planar movement of the lander craft
# Hover Angular. Minimize the rotational movement of ht lander craft
# Upright. Minimize the angle of the lander craft
# Goal Distance. Minimize distance between lander craft and pad
#
# Extrinsic Rewards:
# Success: Did the lander land successfully (1 or 0)

def touch(state):
    """
    Auxiliary reward for touching lander legs on the ground
    :param state: (list) state of lunar lander
    :return: (float) reward
    """
    left_contact = state[6]  # 1.0 if self.legs[0].ground_contact else 0.0
    right_contact = state[7]  # 1.0 if self.legs[1].ground_contact else 0.0
    reward = left_contact + right_contact
    return reward


def hover_planar(state):
    """
    Auxiliary reward for hovering the lander (minimal planar movement)
    :param state: (list) state of lunar lander
    :return: (float) reward
    """
    x_vel = state[2]  # vel.x * (VIEWPORT_W / SCALE / 2) / FPS
    y_vel = state[3]  # vel.y * (VIEWPORT_H / SCALE / 2) / FPS
    reward = 2.0 - (abs(x_vel) + abs(y_vel))
    return reward


def hover_angular(state):
    """
    Auxiliary reward for hovering the lander (minimal angular movement)
    :param state: (list) state of lunar lander
    :return: (float) reward
    """
    ang_vel = state[5]  # 20.0 * self.lander.angularVelocity / FPS
    reward = 2.0 - abs(ang_vel)
    return reward


def upright(state):
    """
    Auxiliary reward for keeping the lander upright
    :param state: (list) state of lunar lander
    :return: (float) reward
    """
    angle = state[4]  # self.lander.angle
    reward = 2.0 - abs(angle)
    return reward


def goal_distance(state):
    """
    Auxiliary reward for distance from lander to goal
    :param state: (list) state of lunar lander
    :return: (float) reward
    """
    x_pos = state[2]  # (pos.x - VIEWPORT_W / SCALE / 2) / (VIEWPORT_W / SCALE / 2)
    y_pos = state[3]  # (pos.y - (self.helipad_y + LEG_DOWN / SCALE)) / (VIEWPORT_W / SCALE / 2)
    reward = 2.0 - (abs(x_pos) + abs(y_pos))
    return reward

def w_from_m(m):
    return np.arctanh(np.sqrt(0.95)) / m

def c_prec(v, t, m):
    return np.tanh(np.abs((v - t) * w_from_m(m))) ** 2

def k_from_t(rate):
    return 1.0 - c_prec(rate, 0.0, 0.5)

def r_rot(r, rate):
    return np.minimum(k_from_t(rate) * r, r)

def r_feet(v_i_swing, v_xy_target, rate):
    res = 0
    for i in range(4):
        res += np.dot(v_xy_target, v_i_swing[i])
    res /= 4
    return r_rot(res, rate)

def r_torso(v_xy_target, v_xy, rate):
    return r_rot(np.dot(v_xy_target, v_xy), rate)

def r_up(fi_t, theta_t):
    return 1 - c_prec(np.sqrt(fi_t ** 2 + theta_t ** 2), 0.0, 0.4)


def r_walk(state):
    obs_size = 125
    num_stacks = 4
    v_xy = np.array([state[obs_size * num_stacks + 2], state[obs_size * num_stacks + 4]], dtype=np.double)
    v_xy_target = np.array([0, 1], dtype=np.double)

    d_F = np.array([-3, -1], dtype=np.double)

    d_fi = np.zeros((4, 2), dtype=np.double)

    for i in range(4):
        d_fi[i] = np.array([state[obs_size * num_stacks + 90 + i * 8 + 0], state[obs_size * num_stacks + 90 + i * 8 + 2]], dtype=np.double)

    v_i_swing = np.zeros((4, 2), dtype=np.double)

    for i in range(4):
        v_i_swing[i] = d_fi[i] - d_F

    g_z = state[obs_size * num_stacks + 9]

    fi_t = state[obs_size * num_stacks + 6]
    theta_t = state[obs_size * num_stacks + 8]

    return r_torso(v_xy_target, v_xy, g_z) + 0.5 * r_feet(v_i_swing, v_xy_target, g_z) + 0.1 * r_up(fi_t, theta_t)





class TaskScheduler(object):
    """Class defines Scheduler for storing and picking tasks"""

    def __init__(self):
        self.aux_rewards = [r_walk]

        # Number of tasks is number of auxiliary tasks plus the main task
        self.num_tasks = len(self.aux_rewards) + 1

        # Internal tracking variable for current task, and set of tasks
        self.current_task = 0
        self.current_set = set()

    def reset(self):
        self.current_set = set()

    def sample(self):
        self.current_task = random.randint(0, self.num_tasks-1)
        self.current_set.add(self.current_task)

    def reward(self, state, main_reward):
        reward_vector = []
        for task_reward in self.aux_rewards:
            reward_vector.append(task_reward(state))
        # Append main task reward
        reward_vector.append(main_reward)
        return reward_vector