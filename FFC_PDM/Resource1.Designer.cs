﻿//------------------------------------------------------------------------------
// <auto-generated>
//     이 코드는 도구를 사용하여 생성되었습니다.
//     런타임 버전:4.0.30319.42000
//
//     파일 내용을 변경하면 잘못된 동작이 발생할 수 있으며, 코드를 다시 생성하면
//     이러한 변경 내용이 손실됩니다.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FFC_PDM {
    using System;
    
    
    /// <summary>
    ///   지역화된 문자열 등을 찾기 위한 강력한 형식의 리소스 클래스입니다.
    /// </summary>
    // 이 클래스는 ResGen 또는 Visual Studio와 같은 도구를 통해 StronglyTypedResourceBuilder
    // 클래스에서 자동으로 생성되었습니다.
    // 멤버를 추가하거나 제거하려면 .ResX 파일을 편집한 다음 /str 옵션을 사용하여 ResGen을
    // 다시 실행하거나 VS 프로젝트를 다시 빌드하십시오.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Resource1 {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resource1() {
        }
        
        /// <summary>
        ///   이 클래스에서 사용하는 캐시된 ResourceManager 인스턴스를 반환합니다.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("FFC_PDM.Resource1", typeof(Resource1).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   이 강력한 형식의 리소스 클래스를 사용하여 모든 리소스 조회에 대해 현재 스레드의 CurrentUICulture 속성을
        ///   재정의합니다.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   &quot;datetime&quot;,&quot;machineID&quot;,&quot;errorID&quot;
        ///2015-01-03 07:00:00,1,&quot;error1&quot;
        ///2015-01-03 20:00:00,1,&quot;error3&quot;
        ///2015-01-04 06:00:00,1,&quot;error5&quot;
        ///2015-01-10 15:00:00,1,&quot;error4&quot;
        ///2015-01-22 10:00:00,1,&quot;error4&quot;
        ///2015-01-25 15:00:00,1,&quot;error4&quot;
        ///2015-01-27 04:00:00,1,&quot;error1&quot;
        ///2015-03-03 22:00:00,1,&quot;error2&quot;
        ///2015-03-05 06:00:00,1,&quot;error1&quot;
        ///2015-03-20 18:00:00,1,&quot;error1&quot;
        ///2015-03-26 01:00:00,1,&quot;error2&quot;
        ///2015-03-31 23:00:00,1,&quot;error1&quot;
        ///2015-04-19 06:00:00,1,&quot;error2&quot;
        ///2015-04-19 06:00:00,1,&quot;error3&quot;
        ///2015-04-29 19:00:00,1,&quot;error4&quot;[나머지 문자열은 잘림]&quot;;과(와) 유사한 지역화된 문자열을 찾습니다.
        /// </summary>
        public static string PdM_errors {
            get {
                return ResourceManager.GetString("PdM_errors", resourceCulture);
            }
        }
        
        /// <summary>
        ///   &quot;datetime&quot;,&quot;machineID&quot;,&quot;failure&quot;
        ///2015-01-05 06:00:00,1,&quot;comp4&quot;
        ///2015-03-06 06:00:00,1,&quot;comp1&quot;
        ///2015-04-20 06:00:00,1,&quot;comp2&quot;
        ///2015-06-19 06:00:00,1,&quot;comp4&quot;
        ///2015-09-02 06:00:00,1,&quot;comp4&quot;
        ///2015-10-17 06:00:00,1,&quot;comp2&quot;
        ///2015-12-16 06:00:00,1,&quot;comp4&quot;
        ///2015-03-19 06:00:00,2,&quot;comp1&quot;
        ///2015-03-19 06:00:00,2,&quot;comp2&quot;
        ///2015-04-18 06:00:00,2,&quot;comp2&quot;
        ///2015-12-29 06:00:00,2,&quot;comp2&quot;
        ///2015-01-07 06:00:00,3,&quot;comp2&quot;
        ///2015-02-06 06:00:00,3,&quot;comp1&quot;
        ///2015-07-21 06:00:00,3,&quot;comp2&quot;
        ///2015-10-04 06:00:00,3,&quot;comp2&quot;
        ///2015-12-03 06[나머지 문자열은 잘림]&quot;;과(와) 유사한 지역화된 문자열을 찾습니다.
        /// </summary>
        public static string PdM_failures {
            get {
                return ResourceManager.GetString("PdM_failures", resourceCulture);
            }
        }
        
        /// <summary>
        ///   &quot;machineID&quot;,&quot;model&quot;,&quot;age&quot;
        ///1,&quot;model3&quot;,18
        ///2,&quot;model4&quot;,7
        ///3,&quot;model3&quot;,8
        ///4,&quot;model3&quot;,7
        ///5,&quot;model3&quot;,2
        ///6,&quot;model3&quot;,7
        ///7,&quot;model3&quot;,20
        ///8,&quot;model3&quot;,16
        ///9,&quot;model4&quot;,7
        ///10,&quot;model3&quot;,10
        ///11,&quot;model2&quot;,6
        ///12,&quot;model3&quot;,9
        ///13,&quot;model1&quot;,15
        ///14,&quot;model3&quot;,1
        ///15,&quot;model3&quot;,14
        ///16,&quot;model1&quot;,3
        ///17,&quot;model1&quot;,14
        ///18,&quot;model3&quot;,15
        ///19,&quot;model3&quot;,17
        ///20,&quot;model2&quot;,16
        ///21,&quot;model2&quot;,14
        ///22,&quot;model1&quot;,14
        ///23,&quot;model1&quot;,17
        ///24,&quot;model1&quot;,20
        ///25,&quot;model4&quot;,16
        ///26,&quot;model3&quot;,3
        ///27,&quot;model3&quot;,9
        ///28,&quot;model4&quot;,1
        ///29,&quot;model4&quot;,3
        ///30,&quot;model3&quot;,20
        ///31,&quot;model1&quot;,11
        ///32,&quot;model4&quot;,[나머지 문자열은 잘림]&quot;;과(와) 유사한 지역화된 문자열을 찾습니다.
        /// </summary>
        public static string PdM_machines {
            get {
                return ResourceManager.GetString("PdM_machines", resourceCulture);
            }
        }
        
        /// <summary>
        ///   &quot;datetime&quot;,&quot;machineID&quot;,&quot;comp&quot;
        ///2014-06-01 06:00:00,1,&quot;comp2&quot;
        ///2014-07-16 06:00:00,1,&quot;comp4&quot;
        ///2014-07-31 06:00:00,1,&quot;comp3&quot;
        ///2014-12-13 06:00:00,1,&quot;comp1&quot;
        ///2015-01-05 06:00:00,1,&quot;comp4&quot;
        ///2015-01-05 06:00:00,1,&quot;comp1&quot;
        ///2015-01-20 06:00:00,1,&quot;comp3&quot;
        ///2015-01-20 06:00:00,1,&quot;comp1&quot;
        ///2015-02-04 06:00:00,1,&quot;comp4&quot;
        ///2015-02-04 06:00:00,1,&quot;comp3&quot;
        ///2015-02-19 06:00:00,1,&quot;comp3&quot;
        ///2015-03-06 06:00:00,1,&quot;comp1&quot;
        ///2015-03-21 06:00:00,1,&quot;comp1&quot;
        ///2015-04-05 06:00:00,1,&quot;comp3&quot;
        ///2015-04-20 06:00:00,1,&quot;comp2&quot;
        ///2015-05-05 06:00[나머지 문자열은 잘림]&quot;;과(와) 유사한 지역화된 문자열을 찾습니다.
        /// </summary>
        public static string PdM_maint {
            get {
                return ResourceManager.GetString("PdM_maint", resourceCulture);
            }
        }
        
        /// <summary>
        ///   &quot;datetime&quot;,&quot;machineID&quot;,&quot;volt&quot;,&quot;rotate&quot;,&quot;pressure&quot;,&quot;vibration&quot;
        ///2015-01-01 06:00:00,1,176.217853015625,418.504078221616,113.077935462083,45.0876857639276
        ///2015-01-01 07:00:00,1,162.87922289706,402.747489565395,95.4605253823187,43.4139726834815
        ///2015-01-01 08:00:00,1,170.989902405567,527.349825452291,75.2379048586662,34.1788471214451
        ///2015-01-01 09:00:00,1,162.462833264092,346.149335043074,109.248561276504,41.1221440884256
        ///2015-01-01 10:00:00,1,157.61002119306,435.376873016938,111.886648210168,25.99051099820[나머지 문자열은 잘림]&quot;;과(와) 유사한 지역화된 문자열을 찾습니다.
        /// </summary>
        public static string PdM_telemetry {
            get {
                return ResourceManager.GetString("PdM_telemetry", resourceCulture);
            }
        }
        
        /// <summary>
        ///   volt,rotate,pressure,vibration,errorID,model,age
        ///191.8731711,382.7366256,100.8936911,37.94021952,0,1,18
        ///182.021908,392.2701871,99.94629329,41.67618411,0,2,7
        ///164.9064664,410.7420748,91.72159297,35.4769705,0,3,8
        ///149.4096957,522.1231175,112.6804986,45.63786969,0,4,7
        ///178.7891968,415.1672978,142.4142734,47.02021034,0,5,2
        ///181.2091761,393.8531968,98.74357764,41.87911972,0,6,7
        ///181.3431133,339.7718811,107.4882337,37.93320846,0,7,20
        ///168.4391841,438.340305,89.90414074,32.15452413,0,8,16
        ///176.0074799,414.778969[나머지 문자열은 잘림]&quot;;과(와) 유사한 지역화된 문자열을 찾습니다.
        /// </summary>
        public static string PdM_telemetry_latest {
            get {
                return ResourceManager.GetString("PdM_telemetry_latest", resourceCulture);
            }
        }
        
        /// <summary>
        ///   datetime,machineID,volt,rotate,pressure,vibration
        ///2015-01-01 6:00,1,176.217853,418.5040782,113.0779355,45.08768576
        ///2015-01-01 7:00,1,162.8792229,402.7474896,95.46052538,43.41397268
        ///2015-01-01 8:00,1,170.9899024,527.3498255,75.23790486,34.17884712
        ///2015-01-01 9:00,1,162.4628333,346.149335,109.2485613,41.12214409
        ///2015-01-01 10:00,1,157.6100212,435.376873,111.8866482,25.990511
        ///2015-01-01 11:00,1,172.5048392,430.3233621,95.92704169,35.65501733
        ///2015-01-01 12:00,1,156.5560306,499.0716231,111.7556843,42.7539[나머지 문자열은 잘림]&quot;;과(와) 유사한 지역화된 문자열을 찾습니다.
        /// </summary>
        public static string PdM_telemetry_no_duplicates {
            get {
                return ResourceManager.GetString("PdM_telemetry_no_duplicates", resourceCulture);
            }
        }
    }
}
