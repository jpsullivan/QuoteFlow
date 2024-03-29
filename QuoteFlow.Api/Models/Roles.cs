﻿using System;

namespace QuoteFlow.Api.Models
{
    [Flags]
    public enum Roles
    {
        None = 0,
        Anonymous = 1 << 1,
        Authenticated = 1 << 2,

        Exceptions = 1 << 3,
        ExceptionsAdmin = 1 << 4,

        HAProxy = 1 << 5,
        HAProxyAdmin = 1 << 6,

        SQL = 1 << 7,
        SQLAdmin = 1 << 8,

        Elastic = 1 << 9,
        ElasticAdmin = 1 << 10,

        Redis = 1 << 11,
        RedisAdmin = 1 << 12,

        Dashboard = 1 << 13,
        DashboardAdmin = 1 << 14,

        InternalRequest = 1 << 19,

        GlobalAdmin = 1 << 20
    }

    public class Role
    {
        public int Key { get; set; }

        public string Name { get; set; }
    }
}