# PhotosForGrandpa
My grandpa is getting a bit older and has difficulty downloading his pictures and videos from Google Photos and sorting them in the correct folders on his PC. This project tries to make this easier for him by automating the task.

Some time ago I created a nice manual for him with screenshots that he printed and uses whenever he wants to put photos/videos from his phone on his computer. Together with the video from his actual video camera he uses Pinnacle Studio to create a little DVD with some transitions and music so he can watch it on TV with my grandma. _(I tried to teach him things like Windows Movie Maker since his Pinnacle is getting old and is quite complicated, but somehow he prefers Pinnacle over Windows Movie Maker and does not want to learn another program.)_

The manual is not really working out for him anymore and this makes me a bit sad. That is why I created this project to make it easier for him!

## What does he usually need to do?

1. Download the photos/videos from https://photos.google.com by selecting the ones he wants. This will automatically get downloaded to the **Downloads** folder on his Windows 8 PC.
2. Unzip the file by right clicking on it and choosing **Extract to Photos/**

_If he is exporting pictures:_

3. Go to his desktop to open the **Pictures** folder.
4. Create a new folder with a name that describes the pictures he will put in there.
5. Go back to the **Downloads** folder.
6. Open the just unzipped **Photos** folder from step 2.
7. Find all the pictures and copy them by selecting them and right-clicking and selecting **Copy**. The photos are on the top and videos on the bottom, but it is difficult for him to remember the difference between them sometimes.
8. Go back to the **Pictures** folder on his desktop.
9. Open the created folder from step 4.
10. Paste the pictures there.

_If he is exporting videos:_

11. Go to his desktop to open the **Videos** folder.
12. Create a new folder with a name that describes the videos he will put in there.
13. Go back to the **Downloads** folder.
14. Open the just unzipped **Photos** folder from step 2.
15. Find all the videos and copy them by selecting them and right-clicking and selecting **Copy**.
16. Go back to the **Videos** folder on his desktop.
17. Open the created folder from step 12.
18. Paste the videos there.
19. Go back to the **Downloads** folder on his Desktop and delete the **Photos** folder and **Photos** zip to avoid clutter. He does not see file extensions so the difference for him is the icon; which is a bundle of books from WinRar and a folder from Windows.

You can see that these are quite a lot of steps. If you are reading this, you might be telling yourself that things could be made MUCH easier by for example using the Windows Explorer sidebar, alt-tab between open windows, cutting instead of copying, using Windows Search to differentiate between photos and videos in the **Photos** folder, I could go on and on. 

But my grandpa is getting old and I am already kind of impressed with the things he knows. And every new thing I teach him will just make things more and more difficult for him. A computer is not easy to use for an old man, that's why we got my grandma an iPad :wink:.


## What does this project aim to accomplish?
While writing this readme, I plan on creating a simple .NET Core console project that asks my grandpa what the name of the folders should become. The reason why I am not creating a GUI for him is because I'm lazy and this is easier. If he has difficulty with the console I might just use .NET Framework and WinForms instead.

My plans are the following:

0. Make my grandpa still download all the pictures himself from https://photos.google.com. He knows very well how to do this. This is **NOT** part of this project.
1. Ask my grandpa for the name of the folders that should be created.
2. Assume the downloaded .zip file is called **Photos.zip** and is present in the **Downloads** folder
3. Unzip that file.
4. Create a folder for his photos in the **Pictures** folder on his PC, named with the input he gave in step 1.
5. Find all the pictures in the folder where are the photos/videos are unzipped in and move them to the created folder in **Pictures**.
6. Repeat step 3 and 4 but now for videos.
7. Delete the .zip and unzipped folder in the **Downloads** folder.
8. Notify him everything is done!
