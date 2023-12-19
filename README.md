# FFC_PDM_WPF_Mini_Project
팀명: FFC(Future Factory Collaborators)

팀장: 윤석희

팀원: 김정관, 성민철

주제: 제조 공정 예측 유지 보수하기


# contents
1. 개요
2. 사용 데이터 및 데이터 분석 계획
3. 사용 기술 및 개발 계획
4. 기능 및 화면 설계

# 1. 개요
반도체, 2차 전지 등 공정 과정의 전압, 진동수, 압력, 회전수를 모니터링하여 고장의 위험도를 감소시킨다. 구체적으로는 데이터 분석을 통해 학습 시킨 머신러닝 모델을 사용하여 고장 위험을 사전에 판별한다. 95% 이상의 정확도를 가진 예측 시스템을 사용하여 고장을 예방한다. 작은 문제를 조기에 해결하면 큰 수리 비용을 피할 수 있고, 예방 정비는 비용 효율적인 방법으로 시스템을 유지할 수 있다. 이를 통하여 전체적인 설비의 안정성을 향상시킬 수 있다.

PLC를 사용하여 전압, 진동수, 압력, 회전수를 정상, 주의, 위험 세가지 기준으로 구분하여 각 상태에 따라 적절한 제어 회로를 작동하여 설비를 자동화한다.



# 2. 사용 데이터 및 데이터 분석
- 사용 데이터

&nbsp;&nbsp;&nbsp;&nbsp;[캐글의 xinjang(Predictive Maintenance)의 PdM 데이터 활용](https://www.kaggle.com/datasets/yuansaijie0604/xinjiang-pm/code)

<details>
<summary>&nbsp;&nbsp;&nbsp;&nbsp; 데이터 예시</summary>

&nbsp;&nbsp;&nbsp;&nbsp;A. PdM_errors.csv

![image](https://github.com/sn50hee/FFC_PDM_Project/assets/139873815/85281b24-7ed6-462d-832e-7f24134a4c59)

&nbsp;&nbsp;&nbsp;&nbsp;B. PdM_failures.csv

![image](https://github.com/sn50hee/FFC_PDM_Project/assets/139873815/fce08720-5f42-4a3d-a250-4a525e2159e4)

&nbsp;&nbsp;&nbsp;&nbsp;C. PdM_machines.csv

![image](https://github.com/sn50hee/FFC_PDM_Project/assets/139873815/1322c73a-8434-4caa-aed6-71865e7cd0bb)

&nbsp;&nbsp;&nbsp;&nbsp;D. PdM_maint.csv

![image](https://github.com/sn50hee/FFC_PDM_Project/assets/139873815/80ae350e-8b76-4d2a-a052-3f7d6b2a1727)

&nbsp;&nbsp;&nbsp;&nbsp;E. PdM_telemetry.csv

![image](https://github.com/sn50hee/FFC_PDM_Project/assets/139873815/f1b56fff-224b-4973-bdb5-ee30d90ee050)

</details>



- 데이터 분석

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;A. 장비 가동 데이터, 장비 정보, 설비 고장 이력 데이터, 경고 이력 데이터, 유지 보수 이력 데이터를 활용하여 고장에 위험을 주는 요소를 선별한다.

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;B. 고장에 위험을 주는 요소는 결과에 얼마나 영향을 미치는지를 기준으로 선별하였다. 아래 사진을 통하여 장비ID(machineID), 회전 수(rotate), 설비 연식(age), 전압(volt), 압력(pressure), 진동 수(vibrationq) 순으로 결과에 영향을 미친다는 것을 확인할 수 있다.

<div align="center">
  <img src="https://github.com/sn50hee/FFC_PDM_Project/assets/139873815/ca42622b-5fc8-44ff-8cba-569f5f137b9a" alt="변수중요도" width="500" height="500">
</div>


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;C. 각각의 고장 위험 요소에 대해 임계값을 설정하고, 해당 값들을 초과할 경우 위험 상태로 판단하도록 분석 기준을 수립한다. 

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;D. 정상 데이터와 고장 데이터를 비교하여 정상 데이터 대비 고장 데이터의 비율이 높아지는 부분을 임계값으로 설정하였다.

<div align="center">
  <img src="https://github.com/sn50hee/FFC_PDM_Project/assets/139873815/20ac3f64-481f-4184-b6bf-e801e68faf3b" alt="임계값" width="50%" height="50%">
</div>

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;F. 머신러닝 모델에 데이터를 학습시켜 고장 위험을 판별할 수 있게 해준다.

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;사용한 데이터셋에서 정상 데이터(875,382개)와 고장 데이터(761개)의 개수 차이가 크기 때문에 정확도만으로는 모델의 성능을 정확하게 평가하기 어렵다. 모든 데이터를 정상으로 예측한 경우 약99.91%의 정확도를 가지게 된다. 그렇기 때문에 4가지의 경우를 나누어서 모델의 성능을 평가하였다. 아래 사진을 통하여 약 95.5%의 정확도를 가진 모델임을 알 수 있다.

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;- 1행 1열 (왼쪽 위): True Negatives (TN) - 실제 정상이면서 모델이 정상으로 예측한 비율

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;- 1행 2열 (오른쪽 위): False Positives (FP) - 실제 정상이지만 모델이 고장으로 예측한 비율

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;- 2행 1열 (왼쪽 아래): False Negatives (FN) - 실제 고장이지만 모델이 정상으로 예측한 비율

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;- 2행 2열 (오른쪽 아래): True Positives (TP) - 실제 고장이면서 모델이 고장으로 예측한 비율

<div align="center">
  <img src="https://github.com/sn50hee/FFC_PDM_Project/assets/139873815/071e316f-ed69-4cc9-95e5-048b8f8b84d4" alt="혼돈행렬" width="50%" height="50%">
</div>


# 3. 사용 기술 및 개발 계획
## 1) 사용 기술

1-1) 데이터 분석

&nbsp;&nbsp;&nbsp;&nbsp;A. 언어: python3.8

&nbsp;&nbsp;&nbsp;&nbsp;B. 라이브러리: numpy1.24.3, pandas2.0.3, joblib

&nbsp;&nbsp;&nbsp;&nbsp;C. 개발 툴: Visual Studio Code 1.84.2


1-2) GUI

&nbsp;&nbsp;&nbsp;&nbsp;A. 언어: C# 11.0

&nbsp;&nbsp;&nbsp;&nbsp;B. Framework: .NET 7.0, WPF

&nbsp;&nbsp;&nbsp;&nbsp;C. 라이브러리: scottplot 4.1.68, XGCommLib(PLC와 통신)


1-3) 데이터 분석 모델과 WPF 연결

&nbsp;&nbsp;&nbsp;&nbsp;A. C# Process 클래스(네임스페이스: System.Diagnostics)

1-4) PLC

&nbsp;&nbsp;&nbsp;&nbsp;A. 언어: LD(Ladder Diagram)



## 2) 개발 계획
A. 개발 기간: 2023.12.20(수) ~ 2023.12.28(목)

B. 상세 계획

<table>
  <tr>
    <th>업무자</th>
    <th>역할</th>
    <th>기간</th>
  </tr>
  <tr>
    <td rowspan="2">성민철</td>
    <td>PLC-WPF 연결 및 양방향 통신</td>
    <td>2023.12.20(수) ~ 2023.12.22(금)</td>
  </tr>
  <tr>
    <td>WPF 구현</td>
    <td>2023.12.26(화) ~ 2023.12.27(수)</td>
  </tr>
  <tr>
    <td rowspan="2">김정관</td>
    <td>PLC LD 프로그램 구현</td>
    <td>2023.12.20(수) ~ 2023.12.22(금)</td>
  </tr>
  <tr>
    <td>WPF 구현</td>
    <td>2023.12.26(화) ~ 2023.12.27(수)</td>
  </tr>
  <tr>
    <td>윤석희</td>
    <td>UI 수정, 버그 수정</td>
    <td>2023.12.20(수) ~ 2023.12.27(수)</td>
  </tr>
  <tr>
    <td>공통</td>
    <td>통합 테스트</td>
    <td>2023.12.27(수) ~ 2023.12.28(목)</td>
  </tr>
</table>

<details>
<summary>C. 클래스 다이어그램</summary>

![png](https://github.com/sn50hee/FFC_PDM_Project/assets/139873815/fb31dadf-1d02-4d3b-8183-3a6e0dffbe5e)

</details>

# 4. 기능 및 UI
## 1) 기능

&nbsp;&nbsp;&nbsp;&nbsp;A) 통계 탭을 통하여 전체 장비의 고장률, 오류 발생률 등 통계 정보를 pie chart로 보여준다.

&nbsp;&nbsp;&nbsp;&nbsp;B) 상세 보기 탭에서는 모델명, 모델ID를 선택할 수 있다.(다중 선택 가능)

&nbsp;&nbsp;&nbsp;&nbsp;C) 상세 보기 탭에서는 원하는 기간을 설정할 수 있다.

&nbsp;&nbsp;&nbsp;&nbsp;D) 선택한 장비와 기간에 대해서 위험 인자에 대한 정보와 고장 횟수, 유지보수 횟수을 보여준다.

&nbsp;&nbsp;&nbsp;&nbsp;E) 위험 인자에 대한 정보는 안정 범위(임계값)와 현재 위험한 장비의 모델ID, 차트(Scatter Plot)가 있다.

&nbsp;&nbsp;&nbsp;&nbsp;F) 차트의 X축은 시간, Y축은 값으로 구성된다.

&nbsp;&nbsp;&nbsp;&nbsp;G) 고장 횟수, 유지보수 횟수는 bar chart로 보여준다

&nbsp;&nbsp;&nbsp;&nbsp;H) 초기값 세팅 탭에서는 장비의 자동화를 위한 초기값을 지정할 수 있다.

&nbsp;&nbsp;&nbsp;&nbsp;I) 자동화를 위한 초기값을 지정할 때 학습 시킨 머신러닝 모델을 사용하여 고장 위험을 알린다.

&nbsp;&nbsp;&nbsp;&nbsp;J) 고장 위험이 높은 값을 입력 시 메시지 박스를 사용하여 안정 범위의 초기값을 유도한다.




## 2) 화면 설계
통계 탭

![슬라이드1](https://github.com/sn50hee/FFC_PDM/assets/139873815/377bf04c-8e34-4230-b52f-07019b02ebce)

![슬라이드2](https://github.com/sn50hee/FFC_PDM/assets/139873815/f2730e07-db1d-4531-837c-7aa70e8ffefd)


상세보기 탭

![슬라이드3](https://github.com/sn50hee/FFC_PDM/assets/139873815/3e9bdb1a-8aad-4b99-83ae-866326bff0ac)

![슬라이드4](https://github.com/sn50hee/FFC_PDM/assets/139873815/61455b1f-24d5-4aa9-b15f-85ff3cf36408)

![슬라이드5](https://github.com/sn50hee/FFC_PDM/assets/139873815/1df1c89a-901a-4b63-9522-e2a121052c77)


초기값 세팅

![프레젠테이션1](https://github.com/sn50hee/FFC_PDM_Project/assets/139873815/64665fca-176c-4a54-aaae-3af15463db3e)

