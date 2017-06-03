#pragma once

#include "../../Common/ParseTextInstances.h"

#include "TunerBankCtrl.h"

class CNotifyManager;
class CEpgDBManager;

class CTunerManager
{
public:
	//�`���[�i�[�ꗗ�̓ǂݍ��݂��s��
	//�߂�l�F
	// TRUE�i�����j�AFALSE�i���s�j
	BOOL ReloadTuner();

	//�`���[�i�[��ID�ꗗ���擾����B
	//�߂�l�F
	// TRUE�i�����j�AFALSE�i���s�j
	//�����F
	// idList			[OUT]�`���[�i�[��ID�ꗗ
	BOOL GetEnumID(
		vector<DWORD>* idList
		) const;

	//�`���[�i�[�\�񐧌���擾����
	//�߂�l�F
	// TRUE�i�����j�AFALSE�i���s�j
	//�����F
	// ctrlMap			[OUT]�`���[�i�[�\�񐧌�̈ꗗ
	// notifyManager	[IN]CTunerBankCtrl�ɓn������
	// epgDBManager		[IN]CTunerBankCtrl�ɓn������
	BOOL GetEnumTunerBank(
		map<DWORD, std::unique_ptr<CTunerBankCtrl>>* ctrlMap,
		CNotifyManager& notifyManager,
		CEpgDBManager& epgDBManager
		) const;

	//�w��T�[�r�X���T�|�[�g���Ă��Ȃ��`���[�i�[�ꗗ���擾����
	//�߂�l�F
	// TRUE�i�����j�AFALSE�i���s�j
	//�����F
	// ONID				[IN]�m�F�������T�[�r�X��ONID
	// TSID				[IN]�m�F�������T�[�r�X��TSID
	// SID				[IN]�m�F�������T�[�r�X��SID
	// idList			[OUT]�`���[�i�[��ID�ꗗ
	BOOL GetNotSupportServiceTuner(
		WORD ONID,
		WORD TSID,
		WORD SID,
		vector<DWORD>* idList
		) const;

	BOOL GetSupportServiceTuner(
		WORD ONID,
		WORD TSID,
		WORD SID,
		vector<DWORD>* idList
		) const;

	BOOL GetCh(
		DWORD tunerID,
		WORD ONID,
		WORD TSID,
		WORD SID,
		DWORD* space,
		DWORD* ch
		) const;

	//�h���C�o���̃`���[�i�[�ꗗ��EPG�擾�Ɏg�p�ł���`���[�i�[���̃y�A���擾����
	BOOL GetEnumEpgCapTuner(
		vector<pair<vector<DWORD>, WORD>>* idList
		) const;

	BOOL IsSupportService(
		DWORD tunerID,
		WORD ONID,
		WORD TSID,
		WORD SID
		) const;

	BOOL GetBonFileName(
		DWORD tunerID,
		wstring& bonFileName
		) const;

protected:
	struct TUNER_INFO {
		wstring bonFileName;
		WORD epgCapMaxOfThisBon;
		vector<CH_DATA4> chList;
	};

	map<DWORD, TUNER_INFO> tunerMap; //�L�[ bonID<<16 | tunerID
};

