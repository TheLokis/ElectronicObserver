
# 74식 전자관측의

현재 열심히 개발중인 칸코레 뷰어 브라우저입니다.


## 구현되있는 기능

![](https://github.com/andanteyk/ElectronicObserver/wiki/media/mainimage2.png)

각 기능 윈도우는 독립되어 자유롭게 도킹하고 탭 화는 자유로운 레이아웃이 구현되어잇습니다.
간단히 설명하자면 아래의 기능들이 내장되있습니다.
**자세한 내용은[Wiki](https://github.com/andanteyk/ElectronicObserver/wiki)를 참조해주세요.
단, 아직 한글 위키가 준비가 안돼 원어페이지로 연결되는 점 양해부탁드립니다.**  

* 내장 브라우저 (스크린 샷, 줌, 음소거 등)
* 함대 (상태 (원정 중 미 정비 등), 制空 전력, 색적 능력)
 - 함선정보 (Lv, HP, 피로도, 보급, 장비 슬롯등)
 - 함대 목록 (전체 함대의 상태를 한눈에 확인할 수 있습니다)
 - 그룹 (필터링 함 딸 정보를 표시)
* 입거 (수리 시간 계산)
* 공창 (개발 결과 보기, 건조 결과 보기 등)
* 사령부 (사령부 정보)
* 나침반 (진로 예측, 적 편성, 획득 자원 등의 이벤트 예측)
* 전투 (전투 예측 · 결과 표시)
* 정보 (중파 이미지 미 회수함 목록, 해역 게이지 잔량 등)
* 임무 (달성 횟수 / 최대 값 표시)
* 도감 (함선 / 장비 도감)
* 장비 목록
* 알림 (원정 · 수리 완료, 대파 진격 경고 등)
* 레코드 (개발 · 건축 · 드롭 함의 기록 등)
* 창 캡처 (다른 프로그램의 창을 캡처)

또한 모든 기능에서 칸코레가 송수신하는 정보에 간섭 조작은 실시하지 않습니다.


## 다운로드

*링크된 페이지를 참조해주세요. [여기](http://thelokis.egloos.com/)*  

일본어판 [ver. 3.1.2 (2018/05/19)](http://bit.ly/2rVaWmJ)을 바탕으로 번역하였습니다.

한국어판을 사용함으로써 발생하는 문제는 모두 번역자인 TheLoki가 관리하므로, 저한테 피드백을 주시면 됩니다.

아래는 원작자가 명시해놓은 라이센스, 개발등에 대한 정보입니다.

## 開発者の皆様へ

[開発のための情報はこちらに掲載しています。](https://github.com/andanteyk/ElectronicObserver/wiki/ForDev)  

[Other/Information/](https://github.com/andanteyk/ElectronicObserver/tree/develop/ElectronicObserver/Other/Information) に艦これのAPIや仕様についての情報を掲載しています。  
ご自由にお持ちください。但し内容は保証しません。  

[ライセンスは MIT License です。](https://github.com/andanteyk/ElectronicObserver/blob/master/LICENSE)  


## 使用しているライブラリ

* [DynamicJson](http://dynamicjson.codeplex.com/) (JSON データの読み書き) - [Ms-PL](https://github.com/andanteyk/ElectronicObserver/blob/master/Licenses/Ms-PL.txt)
* [DockPanel Suite](http://dockpanelsuite.com/) (ウィンドウレイアウト) - [MIT License](https://github.com/andanteyk/ElectronicObserver/blob/master/Licenses/DockPanelSuite.txt)
* [Nekoxy](https://github.com/veigr/Nekoxy) (通信キャプチャ) - [MIT License](https://github.com/andanteyk/ElectronicObserver/blob/master/Licenses/Nekoxy.txt)
    * [TrotiNet](http://trotinet.sourceforge.net/) - [GNU Lesser General Public License v3.0](https://github.com/andanteyk/ElectronicObserver/blob/master/Licenses/LGPL.txt)
        * [log4net](https://logging.apache.org/log4net/) - [Apache License version 2.0](https://github.com/andanteyk/ElectronicObserver/blob/master/Licenses/Apache.txt)
* [SwfExtractor](https://github.com/andanteyk/SwfExtractor) (swf からファイル抽出) - [MIT License](https://github.com/andanteyk/ElectronicObserver/blob/master/Licenses/SwfExtractor.txt)
	* [LZMA SDK (Software Development Kit)](http://www.7-zip.org/sdk.html) - Public Domain


## 連絡先など

* 원본 배포 사이트:[ブルネイ工廠電気実験部](http://electronicobserver.blog.fc2.com/)
* 개발:[Andante](https://twitter.com/andanteyk)
* 번역:[TheLoki](http://thelokis.egloos.com/)
