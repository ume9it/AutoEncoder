set auc=%~1
set avs=%~2
set Output=%~3
set AviUtlPath=%~4

rem ~~~~~~CMカットの情報が載ったavsをAviUtlで開き、mp4にエンコードしてからAviUtlを閉じる~~~~~~
start /b "" %AviUtlPath%
timeout /t 2
call "%auc%\auc_open" "%avs%" & call "%auc%\auc_plugout" 0 "%Output%" & call "%auc%\auc_wait" & call "%auc%\auc_exit"