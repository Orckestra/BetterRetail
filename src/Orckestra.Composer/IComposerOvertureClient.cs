using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ServiceStack;

namespace Orckestra.Composer
{
    public interface IComposerOvertureClient
    {
        void Send(IReturnVoid requestDto);
        TResponse Send<TResponse>(IReturn<TResponse> request);
        void Send(IReturnVoid requestDto, string httpMethod);
        Task<TResponse> SendAsync<TResponse>(IReturn<TResponse> requestDto);
        Task<HttpWebResponse> SendAsync(IReturnVoid requestDto);
        void SendAllOneWay(IEnumerable<IReturnVoid> request);
        List<TResponse> SendAll<TResponse>(IEnumerable<IReturn<TResponse>> request);
        Task<List<TResponse>> SendAllAsync<TResponse>(IEnumerable<IReturn<TResponse>> request);
    }

}
