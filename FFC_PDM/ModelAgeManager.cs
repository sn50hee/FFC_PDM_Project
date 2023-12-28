using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFC_PDM
{
    internal class ModelAgeManager
    {
        // 각 모델의 마지막 업데이트 시간을 저장하는 dictionary
        private Dictionary<int, DateTime> modelLastUpdateTimes = new Dictionary<int, DateTime>();

        public int GetAgeForModel(int modelId)
        {
            // 현재 시간 저장
            DateTime currentTime = DateTime.Now;

            // 모델이 modelLastUpdateTimes에 없으면
            if (!modelLastUpdateTimes.ContainsKey(modelId))
            {
                // 업데이트 시간을 현재시간으로 선언
                modelLastUpdateTimes[modelId] = currentTime;
            }

            // 모델의 마지막 업데이트 시간 갖고오기
            DateTime lastUpdateTime = modelLastUpdateTimes[modelId];

            // 경과한 연수 계산(윤년때문에 365.25
            int passedYears = (int)((currentTime - lastUpdateTime).TotalDays / 365.25);

            // 현재 나이 계산
            int currentAge = GetInitialAgeForModel(modelId) + passedYears;

            // 모델 마지막 업데이트 시간을 현재시간으로 업데이트
            modelLastUpdateTimes[modelId] = currentTime;

            // 계산된 현재 나이 반환
            return currentAge;
        }

        private int GetInitialAgeForModel(int modelId)
        {
            switch (modelId)
            {
                case 1:
                    return 26;
                case 2:
                    return 15;
                case 3:
                    return 16;
                case 4:
                    return 15;
                case 5:
                    return 10;
                case 6:
                    return 15;
                case 7:
                    return 28;
                case 8:
                    return 24;
                default:
                    return 0;
            }
        }
    }
}
