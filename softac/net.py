import torch
import torch.nn as nn
import torch.nn.functional as F
from torch.distributions import Normal

class actor(nn.Module):
    def __init__(self, input_size):
        super(actor, self).__init__()
        self.fc1 = nn.Linear(input_size, 256)
        self.fc1norm = nn.LayerNorm(256)
        self.fc2 = nn.Linear(256, 256)

        self.head = nn.Linear(256, 100)
        self.out1 = nn.Linear(100, 20)
        self.out2 = nn.Linear(100, 20)

        self.clamp = nn.Hardtanh(min_val=0.3, max_val=1.0)

    def forward(self, input):
        x = self.fc1norm(F.elu(self.fc1(input)))
        x = F.elu(self.fc2(x))

        x = F.elu(self.head(x))
        y = F.elu(self.out2(x))
        z = F.elu(self.out1(x))

        z = self.clamp(F.tanh(z))
        y = F.tanh(y)

        #x[:, 30:] = self.clamp(x[:, 30:])
        return Normal(y, z)



class critic(nn.Module):
    def __init__(self, input_size):
        super(critic, self).__init__()
        self.fc1 = nn.Linear(input_size, 400)
        self.fc1norm = nn.LayerNorm(400)
        self.fc2 = nn.Linear(400, 400)

        self.head = nn.Linear(400, 200)
        self.out = nn.Linear(200, 1)

    def forward(self, input, actions):
        x = torch.cat([input, actions], axis=1)
        x = self.fc1norm(F.elu(self.fc1(x)))
        x = F.elu(self.fc2(x))

        x = F.elu(self.head(x))
        x = F.elu(self.out(x))
        return x
