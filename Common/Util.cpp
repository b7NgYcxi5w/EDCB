#include "stdafx.h"
#include "Util.h"

BOOL _CreateDirectory( LPCTSTR lpPathName )
{
	BOOL bRet = FALSE;
	if( _tcslen(lpPathName) > 2 ){
		vector<TCHAR> createPath(lpPathName, lpPathName + _tcslen(lpPathName) + 1);
		
		for (int i = 2; createPath[i] != _T('\0'); i++) {
			if (createPath[i] == _T('\\') || createPath[i+1] == _T('\0')) {
				TCHAR c = createPath[i+1];
				createPath[i+1] = _T('\0');
				if ( GetFileAttributes(&createPath.front()) == 0xFFFFFFFF ) {
					bRet = ::CreateDirectory( &createPath.front(), NULL );
				}
				createPath[i+1] = c;
			}
		}
	}

	return bRet;
}

HANDLE _CreateDirectoryAndFile( LPCTSTR lpFileName, DWORD dwDesiredAccess, DWORD dwShareMode, LPSECURITY_ATTRIBUTES lpsa, DWORD dwCreationDisposition, DWORD dwFlagsAndAttributes, HANDLE hTemplateFile )
{
	HANDLE hFile =  ::CreateFile( lpFileName, dwDesiredAccess, dwShareMode, lpsa, dwCreationDisposition, dwFlagsAndAttributes, hTemplateFile );
	if( hFile == INVALID_HANDLE_VALUE ){
		const TCHAR* p = _tcsrchr(lpFileName, _T('\\'));
		if( p != NULL ){
			vector<TCHAR> dirPath(lpFileName, p + 1);
			dirPath.back() = _T('\0');
			_CreateDirectory(&dirPath.front());
			hFile =  ::CreateFile( lpFileName, dwDesiredAccess, dwShareMode, lpsa, dwCreationDisposition, dwFlagsAndAttributes, hTemplateFile );
		}
	}
	return hFile;
}

BOOL _GetDiskFreeSpaceEx(
  LPCTSTR lpDirectoryName,                 // �f�B���N�g����
  PULARGE_INTEGER lpFreeBytesAvailable,    // �Ăяo���������p�ł���o�C�g��
  PULARGE_INTEGER lpTotalNumberOfBytes,    // �f�B�X�N�S�̂̃o�C�g��
  PULARGE_INTEGER lpTotalNumberOfFreeBytes // �f�B�X�N�S�̂̋󂫃o�C�g��
)
{
	TCHAR szVolumePathName[MAX_PATH] = _T("");
	if( GetVolumePathName( lpDirectoryName, szVolumePathName, MAX_PATH) == FALSE ){
		return GetDiskFreeSpaceEx( lpDirectoryName, lpFreeBytesAvailable, lpTotalNumberOfBytes, lpTotalNumberOfFreeBytes );
	}
	TCHAR szMount[MAX_PATH] = _T("");
	if( GetVolumeNameForVolumeMountPoint(szVolumePathName, szMount, MAX_PATH) == FALSE ){
		return GetDiskFreeSpaceEx( szVolumePathName, lpFreeBytesAvailable, lpTotalNumberOfBytes, lpTotalNumberOfFreeBytes );
	}
	return GetDiskFreeSpaceEx( szMount, lpFreeBytesAvailable, lpTotalNumberOfBytes, lpTotalNumberOfFreeBytes );
}

void GetLastErrMsg(DWORD err, wstring& msg)
{
	LPVOID lpMsgBuf;
	if( FormatMessageW(
		FORMAT_MESSAGE_ALLOCATE_BUFFER|FORMAT_MESSAGE_FROM_SYSTEM|FORMAT_MESSAGE_IGNORE_INSERTS,
		NULL,
		err,
		MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
		(LPWSTR)&lpMsgBuf,
		0,
		NULL) == 0 ){
		msg.clear();
		return;
	}
	msg = (LPWSTR)lpMsgBuf;
	LocalFree( lpMsgBuf );
}

std::basic_string<TCHAR> GetPrivateProfileToString(LPCTSTR lpAppName, LPCTSTR lpKeyName, LPCTSTR lpDefault, LPCTSTR lpFileName)
{
	TCHAR szBuff[512];
	DWORD n = GetPrivateProfileString(lpAppName, lpKeyName, lpDefault, szBuff, 512, lpFileName);
	if( n >= 510 ){
		vector<TCHAR> buff(512);
		do{
			buff.resize(buff.size() * 2);
			n = GetPrivateProfileString(lpAppName, lpKeyName, lpDefault, &buff.front(), (DWORD)buff.size(), lpFileName);
		}while( n >= buff.size() - 2 && buff.size() < 1024 * 1024 );
		return std::basic_string<TCHAR>(buff.begin(), buff.begin() + n);
	}
	return std::basic_string<TCHAR>(szBuff, szBuff + n);
}

BOOL WritePrivateProfileInt(LPCTSTR lpAppName, LPCTSTR lpKeyName, int value, LPCTSTR lpFileName)
{
	TCHAR sz[32];
	_stprintf_s(sz, TEXT("%d"), value);
	return WritePrivateProfileString(lpAppName, lpKeyName, sz, lpFileName);
}
