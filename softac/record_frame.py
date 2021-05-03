import cv2
import numpy as np
import pyautogui
from time import time


# function for recording screen inside specified frame
# while specified condition is True
def record_frame(left, top, width, height,
                 fps, output_file_path,
                 condition_callback,
                 action_callback=None):

    # define the codec
    fourcc = cv2.VideoWriter_fourcc(*'XVID')

    # create the video write object
    writer = cv2.VideoWriter(
        filename=output_file_path,
        fourcc=fourcc,
        fps=fps,
        frameSize=(width, height)
    )

    print('Recording...')

    # while condition is True
    frame_id = 1
    while condition_callback():
        print('frame id: %s' % frame_id)
        frame_id += 1
        # making specified action
        if action_callback:
            action_callback()
        # make a screenshot
        img = pyautogui.screenshot(region=(left, top, width, height))
        # convert these pixels to a proper numpy array to work with OpenCV
        frame = np.array(img)
        # convert colors from BGR to RGB
        frame = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
        # write the frame
        writer.write(frame)

    # make sure everything is closed when exited
    cv2.destroyAllWindows()
    writer.release()

    print('Saved in "%s"' % output_file_path)


# usage example
if __name__ == '__main__':
    # frame recording parameters
    left, top, width, height = 0, 0, 500, 1500
    fps = 3
    output_file_path = 'results/frame_record.avi'

    # dummy condition callback: write only five seconds
    start_time = time()
    condition_callback = lambda: time() < start_time + 5

    # running frame recording
    record_frame(left, top, width, height,
                 fps, output_file_path,
                 condition_callback)
