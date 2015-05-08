using System;
using System.ServiceModel;

namespace server
{
    [ServiceContract(Name = "MyService", Namespace = "urn://com.nbraceit/wcfnsb/myservice")]
    public interface IMyService
    {
        [OperationContract]
        void Test(DateTime timestamp);
    }
}