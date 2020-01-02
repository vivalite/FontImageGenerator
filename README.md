# Training Image Generator for CNN Based Image Chinese Font Finder

Font image generator support generating training image with random background and random font intensity.

Also including script to train the model (VGG19 based) and convert to tensorflow pb format which is required by OpenCV DNN module.

## Note:
* Only support GB2312 / simplified Chinese at the moment.
* Require LabelMe training dataset as the source for background image (http://groups.csail.mit.edu/vision/LabelMe/Benchmarks/spain/training.tar.gz)
