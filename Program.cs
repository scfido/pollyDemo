using System;
using Polly;

namespace pollyDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Polly Demo");
            MixedPolicy();
        }

        static int Compute()
        {
            var zero = 0;
            return 1 / zero;
        }

        private static void Retry()
        {
            try
            {
                var retryTwoTimesPolicy = Policy
                        .Handle<DivideByZeroException>()
                        .Retry(3, (ex, count) =>
                        {
                            Console.WriteLine("执行失败! 重试次数 {0}", count);
                            Console.WriteLine("异常来自 {0}", ex.GetType().Name);
                        });

                retryTwoTimesPolicy.Execute(Compute);
            }
            catch (DivideByZeroException e)
            {
                Console.WriteLine($"Excuted Failed,Message: ({e.Message})");

            }
        }

        private static void Fallback()
        {
            var fallBackPolicy = Policy<int>
                    .Handle<Exception>()
                    .Fallback(0);

            var fallBack = fallBackPolicy.Execute(Compute);
            Console.WriteLine($"除零返回：{fallBack}");
        }

        private static void MixedPolicy()
        {
            var fallBackPolicy = Policy<int>
                    .Handle<Exception>()
                    .Fallback(0);

            var politicaWaitAndRetry =Policy<int>
                    .Handle<Exception>()
                    .Retry(3, (ex, count) =>
                    {
                        Console.WriteLine("执行失败! 重试次数 {0}", count);
                        Console.WriteLine("异常来自 {0}", ex.GetType().Name);
                    });

            var mixedPolicy = Policy.Wrap(fallBackPolicy, politicaWaitAndRetry);
            var mixedResult = mixedPolicy.Execute(Compute);
            Console.WriteLine($"执行结果: {mixedResult}");
        }
    }
}
