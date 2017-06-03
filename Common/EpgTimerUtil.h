#ifndef __EPG_TIMER_UTIL_H__
#define __EPG_TIMER_UTIL_H__

#include "StructDef.h"
#include "EpgDataCap3Def.h"

//�`�����l����__int64�Ƃ��ăL�[�ɂ���
static inline LONGLONG _Create64Key(WORD onid, WORD tsid, WORD sid) { return sid | (DWORD)tsid << 16 | (LONGLONG)onid << 32; }
//EventID��unsigned __int64�Ƃ��ăL�[�ɂ���
static inline ULONGLONG _Create64Key2(WORD onid, WORD tsid, WORD sid, WORD eid) { return eid | (DWORD)sid << 16 | (ULONGLONG)tsid << 32 | (ULONGLONG)onid << 48; }
//CRC32�����Ƃ߂�
unsigned long CalcCrc32(int n, const BYTE* c);
//MJD->FILETIME�ϊ�
FILETIME MJDtoFILETIME(DWORD mjd, DWORD bcdTime = 0);

//ini�t�@�C������\�z�r�b�g���[�g���擾����
DWORD GetBitrateFromIni(WORD onid, WORD tsid, WORD sid);

//EPG����Text�ɕϊ�
wstring ConvertEpgInfoText(const EPGDB_EVENT_INFO* info, const wstring* serviceName = NULL, const wstring* extraText = NULL);
wstring ConvertEpgInfoText2(const EPGDB_EVENT_INFO* info, const wstring& serviceName);

//�t�H���_�p�X������ۂ̃h���C�u�p�X���擾
void GetChkDrivePath(wstring directoryPath, wstring& mountPath);

void GetGenreName(BYTE nibble1, BYTE nibble2, wstring& name);
void GetComponentTypeName(BYTE content, BYTE type, wstring& name);

void ConvertEpgInfo(WORD onid, WORD tsid, WORD sid, const EPG_EVENT_INFO* src, EPGDB_EVENT_INFO* dest);

class CEpgEventInfoAdapter
{
public:
	//EPGDB_EVENT_INFO���Q�Ƃ���EPG_EVENT_INFO���\�z����
	EPG_EVENT_INFO Create(EPGDB_EVENT_INFO* ref);
private:
	EPG_SHORT_EVENT_INFO shortInfo;
	EPG_EXTENDED_EVENT_INFO extInfo;
	EPG_CONTEN_INFO contentInfo;
	EPG_COMPONENT_INFO componentInfo;
	EPG_AUDIO_COMPONENT_INFO audioInfo;
	vector<EPG_AUDIO_COMPONENT_INFO_DATA> audioList;
	EPG_EVENTGROUP_INFO eventGroupInfo;
	EPG_EVENTGROUP_INFO eventRelayInfo;
};

class CServiceInfoAdapter
{
public:
	//EPGDB_SERVICE_INFO���Q�Ƃ���SERVICE_INFO���\�z����
	SERVICE_INFO Create(const EPGDB_SERVICE_INFO* ref);
private:
	SERVICE_EXT_INFO extInfo;
};

#endif
