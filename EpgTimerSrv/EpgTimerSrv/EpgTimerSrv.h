#pragma once

//�T�[�r�X����p�̃��C��
void WINAPI service_main(DWORD dwArgc, LPWSTR* lpszArgv);

//�T�[�r�X����̃R�}���h�̃R�[���o�b�N
DWORD WINAPI service_ctrl(DWORD dwControl, DWORD dwEventType, LPVOID lpEventData, LPVOID lpContext);

//�T�[�r�X�̃X�e�[�^�X�ʒm�p
void ReportServiceStatus(DWORD dwCurrentState, DWORD dwControlsAccepted, DWORD dwCheckPoint, DWORD dwWaitHint);
