@echo off
rem �\����X�V�̃^�C�~���O�ł���ڂ��J�����_�[�ɗ\����A�b�v���[�h����T���v���o�b�`
rem _EDCBX_DIRECT_
rem _EDCBX_#HIDE_ (#����菜���ƃE�B���h�E��\���ɂȂ�)

rem ���x86�ł�powershell���g�p����(EpgTimer.exe�̃A�[�L�e�N�`���ɍ��킹��)
set PsPath="powershell"
if "%PROCESSOR_ARCHITECTURE%"=="AMD64" set PsPath="%SystemRoot%\SysWOW64\WindowsPowerShell\v1.0\powershell"

rem ���ڎ��s���\����X�V�̂Ƃ�����
if not defined NotifyID set NotifyID=2
if "%NotifyID%"=="2" (
  echo ����ڂ��J�����_�[�ɗ\����A�b�v���[�h���܂��B
  %PsPath% -NoProfile -ExecutionPolicy RemoteSigned -File ".\EdcbSchUploader.ps1" �y���[�UID�ƃp�X���[�h�������Ɏw��z

  rem ���퓊�����͂���pause����菜��
  pause
)
