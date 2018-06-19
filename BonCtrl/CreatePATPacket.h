#pragma once

#include "../Common/EpgTimerUtil.h"
#include "../Common/ErrDef.h"

class CCreatePATPacket
{
public:
	CCreatePATPacket(void);

	//�쐬PAT�̃p�����[�^��ݒ�
	//�����F
	// TSID				[IN]TransportStreamID
	// PIDList			[IN]PMT��PID��ServiceID�̃��X�g
	void SetParam(
		WORD TSID_,
		const vector<pair<WORD, WORD>>& PIDList_
	);

	//�쐬PAT�̃o�b�t�@�|�C���^���擾
	//�߂�l�F
	// TRUE�i�����j�AFALSE�i���s�j
	//�����F
	// buff				[OUT]�쐬����PAT�p�P�b�g�ւ̃|�C���^�i����Ăяo�����܂ŗL���j
	// buffSize			[OUT]buff�̃T�C�Y
	// incrementFlag	[IN]TS�p�P�b�g��Counter���C���N�������g���邩�ǂ����iTRUE:����AFALSE�F���Ȃ��j
	BOOL GetPacket(
		BYTE** buff,
		DWORD* buffSize,
		BOOL incrementFlag = TRUE
	);

	//�쐬PAT�̃o�b�t�@���N���A
	void Clear();

protected:
	void CreatePAT();
	void CreatePacket();
	void IncrementCounter();

protected:
	BYTE version;
	BYTE counter;

	WORD TSID;
	vector<pair<WORD, WORD>> PIDList;

	vector<BYTE> packet;

	vector<BYTE> PSI;
};
