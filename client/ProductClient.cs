﻿using System;
using System.ServiceModel;
using server;

namespace client
{
    public class ProductClient : ClientBase<IMyService>, IMyService
    {
        public void Test(DateTime timestamp)
        {
            Channel.Test(timestamp);
        }
    }
}