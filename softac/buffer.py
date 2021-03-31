import numpy as np
from random import randint
import torch

cuda = torch.device('cuda:0')

class Buffer():
    def __init__(self):
        self.observations = torch.zeros((2, 400, 72)).to(device=cuda)
        self.taken_actions = torch.zeros((2, 400, 20)).to(device=cuda)
        self.rewards = torch.zeros((2, 400, 1)).to(device=cuda)
        self.i1 = 0
        self.max_size = 2
        self.batch_size = 400

    def add_trajectory(self, obss, acts, rews):
        self.observations[self.i1] = obss.detach().clone()
        self.taken_actions[self.i1] = acts.detach().clone()
        self.rewards[self.i1] = rews.detach().clone()
        self.i1 += 1
        self.i1 = self.i1 % self.max_size

    def batch(self, batch_size):
        indeces = np.random.choice(list(range(len(self.observations))), batch_size, False)
        return  self.observations[indeces], self.taken_actions[indeces], self.rewards[indeces]

    def sample_trajectory(self, ind=-1):
        if ind == -1:
            indec = randint(0, len(self.observations) - 1)
        else:
            indec = ind
        return self.observations[indec], self.taken_actions[indec], self.rewards[indec]

    def can_sample(self):
        return self.batch_size >= len(self.observations)

    def clear(self):
        self.observations = np.array([], dtype=np.float32)
        self.taken_actions = np.array([], dtype=np.float32)
        self.rewards = np.array([], dtype=np.float32)
        self.i1 = 0
