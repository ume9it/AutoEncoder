set auc=%~1
set avs=%~2
set Output=%~3
set AviUtlPath=%~4

rem ~~~~~~CM�J�b�g�̏�񂪍ڂ���avs��AviUtl�ŊJ���Amp4�ɃG���R�[�h���Ă���AviUtl�����~~~~~~
start /b "" %AviUtlPath%
timeout /t 2
call "%auc%\auc_open" "%avs%" & call "%auc%\auc_plugout" 0 "%Output%" & call "%auc%\auc_wait" & call "%auc%\auc_exit"