### How to install screen recorder
To install screen recorder package we simply need to clone this repo inside the root of this project:
```bash
git clone https://github.com/Crowbar97/ScreenRecorder.git screen_recorder
```

### How to use screen recorder
To have ability of video recording from target window we first need to import `WindowRecorder` module from `screen_recorder` package in our script:
```python
# for importing screen_recorder modules
sys.path.insert(1, path.join(sys.path[0], path.pardir, 'screen_recorder'))
from window_recorder import WindowRecorder
```

And then we can use `WindowRecorder` in such manner (see example in the `softac/interaction.py`):
```python
rec = WindowRecorder(window_name='Unity', log_dir_path='visual_logs/', log_name='test')
rec.start()
# <any code for rendering content on the target window>
rec.finish()
```
