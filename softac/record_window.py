from record_frame import record_frame
from utils import get_window_geometry, window_is_open, bring2front


# usage example
if __name__ == '__main__':
    # window recording parameters
    window_name = 'Telegram Desktop'
    fps = 1
    output_file_path = 'results/window_record.avi'

    # Step 1: getting geometry of the target window
    window_id, left, top, width, height = get_window_geometry(window_name)

    # Step 2: creating condition callback function
    # that checks whether target window is still open
    condition_callback = lambda: window_is_open(window_id)

    # Step 3: creating action callback function
    # that will bring target window to the front
    # each time we need to make screenshot
    action_callback = lambda: bring2front(window_id)

    # Step 4: running window recording
    record_frame(left, top, width, height,
                 fps, output_file_path,
                 condition_callback,
                 action_callback)