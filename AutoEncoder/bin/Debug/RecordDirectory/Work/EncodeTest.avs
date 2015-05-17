LoadPlugin("\\psf\Home\GitHub\AutoEncoder\AutoEncoder\bin\Debug\Library\DGDecode.dll")

rippedPath = "\\psf\Home\GitHub\AutoEncoder\AutoEncoder\bin\Debug\RecordDirectory"
start = 0
end = -1

movie = MPEG2Source(rippedPath + "EncodeTest.d2v")
audio = DirectShowSource(rippedPath + "EncodeTest.wav")
movie = AudioDub(movie, audio)

movie = ConvertToYV12(movie, interlaced = true)
movie = LanczosResize(movie, 1280, 960, taps = 4)

end = (end < 0 ? movie.FrameCount - 1 : end)
movie = Trim(0,1818) ++ Trim(3608,8085)

return movie
