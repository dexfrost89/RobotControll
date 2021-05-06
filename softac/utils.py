import subprocess

# TODO: implement as a class (SystemManager)
# move deviations to config file
# move cmds to config file


# function for executing shell command
def exec_shell_cmd(cmd):
    ready_cmd = cmd.split()
    # print('ready cmd: %s' % ready_cmd)

    process = subprocess.Popen(ready_cmd, stdout=subprocess.PIPE)
    output, error = process.communicate()

    # print('output: %s' % output)
    # print('error: %s' % error)

    return output, error


# function for correcting window's geometry
def correct_geometry(left, top, width, height):
    scale_factor = 1 / 2

    deviations = {
        'left': 52,
        'top': -9,
        'top_bar': 55,
        'width': -100,
        'height': -100,
    }

    return (
        left * scale_factor + deviations['left'],
        top * scale_factor + deviations['top'] + deviations['top_bar'],
        width + deviations['width'],
        height + deviations['height']
    )


# function for getting information about specified window
# NOTE: window_idx can be any identifier (substring) that
# real window contains in its information string
def get_window_info(window_idx):
    print('Searching for window with identifier "%s"...' % window_idx)
    # specifying cmd for getting info about opened windows
    list_windows_cmd = 'wmctrl -lG'
    # executing cmd and getting raw string output
    output, _ = exec_shell_cmd(list_windows_cmd)
    # splitting by lines
    lines = str(output)[2:-1].split('\\n')
    # finding first line with specidfied window name
    for line in lines:
        # print('Scanning window "%s"...' % line)
        if window_idx in line:
            print('Window with identifier "%s" found!' % window_idx)
            return line
    print('Error: no window containing identifier %s was found!' % window_idx)
    return None


# function for getting window geometry
# like position and dimensions
def get_window_geometry(window_name):
    # getting window info
    window_info = get_window_info(window_name).split()
    # extracting window ID
    window_id = window_info[0]
    print('window_id: %s' % window_id)
    # extracting info about geometry
    start_ind = 2
    left, top, width, height = list(map(int, window_info[start_ind:start_ind + 4]))
    # apply corrections
    left, top, width, height = correct_geometry(left, top, width, height)
    print('window_geometry: %s' % str((left, top, width, height)))
    return window_id, left, top, width, height


# function to check whether window
# with specified id is open
def window_is_open(window_id):
    return get_window_info(window_id) is not None


# function for bringing specified window
# to the front
def bring2front(window_id):
    # specifying cmd for bringing specified window to front
    bring2front_cmd = 'wmctrl -ia %s' % window_id
    # executing cmd and getting raw string output
    exec_shell_cmd(bring2front_cmd)