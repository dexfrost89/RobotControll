import cv2
import numpy as np
import pyautogui
import time

from utils import exec_shell_cmd, get_window_geometry, bring2front

# script for debugging

# run parameters
window_name = 'Home'

# Step 1: getting geometry of the target window
window_id, left, top, width, height = get_window_geometry(window_name)

# Step 2: bringing target window to the front
# NOTE: this may take some time after the system call
bring2front(window_id)
time.sleep(1)

# Step 3: making screenshot the target window
# by it's position and dimensions
img = pyautogui.screenshot(region=(left, top, width, height))
frame = np.array(img)
frame = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
cv2.imwrite('screenshot.png', frame)