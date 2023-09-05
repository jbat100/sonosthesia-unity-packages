ffmpeg -i "Movie_008.mp4" -itsoffset 0.12 -i "Movie_008.mp4" -map 0:v -map 1:a -c copy "Movie_008_delayed.mp4"
