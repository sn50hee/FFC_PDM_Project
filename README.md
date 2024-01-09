# FFC_PDM_WPF_Project
팀명: FFC(Future Factory Collaborators)

팀원: 김정관, 성민철, 윤석희

주제: 제조 공정 예측 유지 보수 및 자동화


# contents
1. 개요
2. 사용 데이터 및 데이터 분석 계획
3. 사용 기술 및 개발 계획
4. 기능 및 화면 설계

# 1. 개요

본 프로젝트에서는 특정 공정의 사고 위험을 방지하는 소프트웨어를 개발합니다. 구체적으로는 반도체, 2차 전지와 같은 공정 과정의 전압, 진동수, 압력, 회전수를 모니터링하고, 학습된 머신러닝 모델을 사용하여 사전에 고장 위험을 판별할 수 있습니다. 위험 판별 모델은 95% 이상의 정확도를 가집니다. 실시간으로 변화하는 공정 데이터를 모니터링하고 위험성을 사전에 판별하는 것으로 예상치 못한 피해, 또는 대형 사고를 효과적으로 예방하며, 각 설비의 안정성을 향상시킬 수 있습니다. 핵심 기능에 대한 설명은 아래와 같습니다.

1. PLC를 사용하여 전압, 진동수, 압력, 회전수를 정상, 주의, 위험 세가지 기준으로 구분하고, 공정 상태에 따라 PLC 내부 회로(LD 프로그램 구성)를 변경하는 것으로 자동화를 이루었습니다.

2. X-RAY 검사기를 사용하는 품질 검사 과정에서 생성되는 X-RAY 이미지를 학습하여 AI 객체 탐지 분석 모델을 만들었습니다. 불량품 탐지 시 PLC의 회로를 제어하여 생산라인을 변경합니다.

3. 얼굴 인식 및 동공 움직임 분석을 통해 인가된 사용자의 안전한 접근을 보장하며, 사용자의 집중도를 파악하여 설비의 안정성을 높일 수 있습니다.
위 세가지 요소를 통해 생산 과정의 안정성과 효율성을 극대화 할 수 있습니다.


# 2. 사용 데이터 및 데이터 분석
- 사용 데이터

&nbsp;&nbsp;&nbsp;&nbsp;[캐글의 xinjang(Predictive Maintenance)의 PdM 데이터 활용](https://www.kaggle.com/datasets/yuansaijie0604/xinjiang-pm/code)

<details>
<summary>&nbsp;&nbsp;&nbsp;&nbsp; 데이터 예시</summary>

&nbsp;&nbsp;&nbsp;&nbsp;A. PdM_failures.csv

![image](https://github.com/sn50hee/FFC_PDM_Project/assets/139873815/fce08720-5f42-4a3d-a250-4a525e2159e4)

&nbsp;&nbsp;&nbsp;&nbsp;B. PdM_errors.csv: datetime, machineID, errorID

&nbsp;&nbsp;&nbsp;&nbsp;C. PdM_machines.csv: machineID, model, age

&nbsp;&nbsp;&nbsp;&nbsp;D. PdM_maint.csv: datetime, machineID, comp

&nbsp;&nbsp;&nbsp;&nbsp;E. PdM_telemetry.csv: datetime, machineID, volt, rotate, pressure, vibration

</details>

- 데이터 분석

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;A. 장비 가동 데이터, 장비 정보, 설비 고장 이력 데이터, 경고 이력 데이터, 유지 보수 이력 데이터를 활용하여 고장에 위험을 주는 요소를 선별한다.

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;B. 고장에 위험을 주는 요소는 결과에 얼마나 영향을 미치는지를 기준으로 선별하였다. 아래 사진을 통하여 장비ID(machineID), 회전 수(rotate), 설비 연식(age), 전압(volt), 압력(pressure), 진동 수(vibrationq) 순으로 결과에 영향을 미친다는 것을 확인할 수 있다.

<div align="center">
  <img src="https://github.com/sn50hee/FFC_PDM_Project/assets/139873815/ca42622b-5fc8-44ff-8cba-569f5f137b9a" alt="변수중요도" width="500" height="500">
</div>


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;C. 각각의 고장 위험 요소에 대해 임계값을 설정하고, 해당 값들을 초과할 경우 위험 상태로 판단하도록 분석 기준을 수립한다. 

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;D. 정상 데이터와 고장 데이터를 비교하여 정상 데이터 대비 고장 데이터의 비율이 많이 높아지는 부분을 임계값으로 설정하였다.

<div align="center">
  <img src="https://github.com/sn50hee/FFC_PDM_Project/assets/139873815/20ac3f64-481f-4184-b6bf-e801e68faf3b" alt="임계값" width="50%" height="50%">
</div>

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;F. 머신러닝 모델에 데이터를 학습시켜 고장 위험을 판별할 수 있게 해준다.

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;사용한 데이터셋에서 정상 데이터(875,382개)와 고장 데이터(761개)의 개수 차이가 크기 때문에 전체 데이터의 정확도만으로는 모델의 성능을 정확하게 평가하기 어렵다. 그렇기 때문에 4가지의 경우를 나누어서 모델의 성능을 평가하였다. 아래 사진을 통하여 **약 95.5%의 정확도**를 가진 모델임을 알 수 있다.

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;- 1행 1열 (왼쪽 위): True Negatives (TN) - 실제 정상이면서 모델이 정상으로 예측한 비율

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;- 1행 2열 (오른쪽 위): False Positives (FP) - 실제 정상이지만 모델이 고장으로 예측한 비율

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;- 2행 1열 (왼쪽 아래): False Negatives (FN) - 실제 고장이지만 모델이 정상으로 예측한 비율

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;- 2행 2열 (오른쪽 아래): True Positives (TP) - 실제 고장이면서 모델이 고장으로 예측한 비율

<div align="center">
  <img src="https://github.com/sn50hee/FFC_PDM_Project/assets/139873815/071e316f-ed69-4cc9-95e5-048b8f8b84d4" alt="혼돈행렬" width="50%" height="50%">
</div>

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 최종적으로 96% 정확도로 고장 예측이 가능하다.

<br>

- 이미지 학습 데이터

&nbsp;&nbsp;&nbsp;&nbsp;1) [포장 x-ray](https://www.kamp-ai.kr/aidataDetail?AI_SEARCH=&page=1&DATASET_SEQ=12&EQUIP_SEL=&GUBUN_SEL=&FILE_TYPE_SEL=&WDATE_SEL=)
<br>&nbsp;&nbsp;&nbsp;&nbsp;2) [시선 검출](https://www.aihub.or.kr/aihubdata/data/view.do?currMenu=115&topMenu=100&aihubDataSe=data&dataSetSn=71421)
<br>&nbsp;&nbsp;&nbsp;&nbsp;3) 관리자 얼굴 판별: 개발자의 얼굴 데이터(Github ID: kwaaaaan, raysmc517, sn50hee)

# 3. 사용 기술 및 개발 계획
## 1) 사용 기술

<table>
  <tr>
    <th>담당자</th>
    <th>담당 업무</th>
    <th>사용 기술 및 도구</th>
  </tr>
  <tr>
    <td>김정관</td>
    <td>포장 X-RAY 객체 탐지 분석모델 구축, PLC 제어</td>
    <td><b>CV(Object Detection)</b>
      <br>&nbsp;&nbsp;&nbsp;&nbsp;A. 라이브러리: Numpy, Pandas, Matplotlib, Keras, Sklearn, Tensorflow, Ipython, Pydotplus, Os, Graphviz, Random, Subprocess, Sys
      <br>&nbsp;&nbsp;&nbsp;&nbsp;B. 알고리즘: YOLOv3
      <br>&nbsp;&nbsp;&nbsp;&nbsp;C. 개발 툴: Google Colab
    </td>
  </tr>  <tr>
    <td>성민철</td>
    <td>관리자 얼굴 탐지 모델 구축</td>
    <td><b>CV(Object Detection)</b>
      <br>&nbsp;&nbsp;&nbsp;&nbsp;A. 라이브러리: Numpy, Pandas, Matplotlib, Keras, Sklearn, Tensorflow, Ipython, Pydotplus, Os, Graphviz, Random, Subprocess, Sys
      <br>&nbsp;&nbsp;&nbsp;&nbsp;B. 알고리즘: YOLOv5
      <br>&nbsp;&nbsp;&nbsp;&nbsp;C. 개발 툴: Google Colab
    </td>
  </tr>
  <tr>
    <td>윤석희</td>
    <td>관리자 시선 추적 모델 구축, PLC 제어</td>
    <td><b>CV(Object Detection)</b>
      <br>&nbsp;&nbsp;&nbsp;&nbsp;A. 라이브러리: Numpy, Pandas, Matplotlib, Keras, Sklearn, Tensorflow, Ipython, Pydotplus, Os, Graphviz, Random, Subprocess, Sys
      <br>&nbsp;&nbsp;&nbsp;&nbsp;B. 알고리즘: L2CS-Net
      <br>&nbsp;&nbsp;&nbsp;&nbsp;C. 개발 툴: Visual Studio Code
    </td>
  </tr>
  <tr>
    <td>공통</td>
    <td>WPF 화면 구현, 테스트</td>
    <td><b>WPF</b>
      <br>&nbsp;&nbsp;&nbsp;&nbsp;A. 언어: C# 11.0
      <br>&nbsp;&nbsp;&nbsp;&nbsp;B. Framework: .NET 7.0, WPF
      <br>&nbsp;&nbsp;&nbsp;&nbsp;C. 라이브러리: scottplot 4.1.68, XGCommLib(PLC와 통신)
      <br>&nbsp;&nbsp;&nbsp;&nbsp;D. 모델 통신: C# Process 클래스(네임스페이스: System.Diagnostics)
      <br>&nbsp;&nbsp;&nbsp;&nbsp;E. 개발 툴: Visual Studio 2022
      <br><b>PLC</b>
      <br>&nbsp;&nbsp;&nbsp;&nbsp;A. 언어: LD
      <br>&nbsp;&nbsp;&nbsp;&nbsp;B. 개발 툴: XG5000
    </td>
  </tr>
</table>


## 2) 개발 계획
A. 개발 기간: 2024.01.10(수) ~ 2024.01.15(월)

B. 상세 계획

<table>
  <tr>
    <td>김정관</td>
    <td>포장 X-RAY 객체 탐지 분석모델 구축, PLC LD프로그래밍</td>
    <td>2024.01.10(수) ~ 2024.01.12(금), 총 3일</td>
  </tr>  <tr>
    <td>성민철</td>
    <td>관리자 얼굴 탐지 모델 구축</td>
    <td>2024.01.10(수) ~ 2024.01.12(금), 총 3일</td>
  </tr>
  <tr>
    <td>윤석희</td>
    <td>관리자 시선 추적 모델 구축, PLC LD프로그래밍</td>
    <td>2024.01.10(수) ~ 2024.01.12(금), 총 3일</td>
  </tr>
  <tr>
    <td>공통</td>
    <td>WPF 화면 구현, 테스트</td>
    <td>2024.01.12(금) ~ 2024.01.15(월), 총 2일</td>
  </tr>
</table>

<details>
<summary>C. 클래스 다이어그램</summary>

![png](https://github.com/sn50hee/FFC_PDM_Project/assets/139873815/fb31dadf-1d02-4d3b-8183-3a6e0dffbe5e)

</details>

# 4. 화면 설계

<br>1) 공통 사항

&nbsp;&nbsp;&nbsp;&nbsp;A. 최초 접근 시 관리자 얼굴을 등록하여 관리자가 접근 시에만 시스템 사용이 가능합니다.

&nbsp;&nbsp;&nbsp;&nbsp;B. 관리자의 동공을 통한 시선 추적으로 관리자의 집중도를 파악하여 설비의 속도가 자동 조절합니다.

<br><br><br>
2) 통계 탭

&nbsp;&nbsp;&nbsp;&nbsp;A) 실시간 고장 위험 장비 현황, 실시간 장비 가동률, 최근 10건 고장 장비, 오류 횟수를 보여줍니다.

![통계](https://github.com/sn50hee/FFC_PDM_Project/assets/139873815/81ccf564-0f13-449c-8295-a5eaa7e212e8)

<br><br><br>
3) 상세보기 탭

&nbsp;&nbsp;&nbsp;&nbsp;A) 모델명, 모델ID, 기간을 선택할 수 있습니다.

&nbsp;&nbsp;&nbsp;&nbsp;B) 선택한 장비와 기간에 대해서 위험 인자에 대한 정보와 고장 횟수, 유지보수 횟수을 확인할 수 있습니다.

&nbsp;&nbsp;&nbsp;&nbsp;C) 위험 인자는 임계값을 기준으로 위험한 범위를 표시하여 Scatter Plot으로 보여줍니다.

&nbsp;&nbsp;&nbsp;&nbsp;D) 차트의 X축은 시간, Y축은 값으로 구성됩니다.

&nbsp;&nbsp;&nbsp;&nbsp;E) 고장 횟수, 유지보수 횟수는 pie chart로 보여줍니다.

![상세보기1_tnwjd](https://github.com/sn50hee/FFC_PDM_Project/assets/139873815/75c903e9-83a9-4c97-bfe3-5d92f364b8df)

![상세보기2_수정](https://github.com/sn50hee/FFC_PDM_Project/assets/139873815/2a1607c1-121d-440b-b13d-4aa31e39019a)

<br><br><br>
4) 자동화 값 설정 탭

&nbsp;&nbsp;&nbsp;&nbsp;A) 장비의 자동화를 위한 초기값을 지정할 수 있습니다.

&nbsp;&nbsp;&nbsp;&nbsp;B) 자동화를 위한 초기값을 지정할 때 학습 시킨 머신러닝 모델을 사용하여 고장 위험을 알립니다.

&nbsp;&nbsp;&nbsp;&nbsp;C) PLC 쓰기 버튼을 클릭하면 설정한 값이 PLC로 전송되며 자동화가 시작됩니다.

![자동화 위험 포함](https://github.com/sn50hee/FFC_PDM_Project/assets/139873815/f6753da6-343a-4f4a-aafc-b7a3d1ccca6a)

<br><br><br>
5) 검사 공정 모니터링 탭

&nbsp;&nbsp;&nbsp;&nbsp;A) 검사 공정에서 불량품이 검출될 시 컨베이어 벨트의 라인을 자동 스위칭하여 불량 판별을 자동화합니다.

&nbsp;&nbsp;&nbsp;&nbsp;B) 현재 X-RAY 화면을 확인할 수 있으며 양품일 때는 화면의 테두리가 녹색으로, 불량품일 때는 화면의 테두리가 빨간색으로 표시됩니다.

&nbsp;&nbsp;&nbsp;&nbsp;C) 하단에는 현재 날짜의 불량품 검출 수를 보여줍니다.

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;정상

![안전](https://github.com/sn50hee/FFC_PDM_Project/assets/139873815/d30d12fe-6eb3-4135-8174-7686f1ba1918)

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;불량품

![위험](https://github.com/sn50hee/FFC_PDM_Project/assets/139873815/e4a96b79-8247-4758-b767-e0350521fe7f)
