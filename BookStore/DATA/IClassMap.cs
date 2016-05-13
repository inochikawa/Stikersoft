using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DATA
{
    public interface IClassMap
    {
        void Save<T>(T obj) where T : IEntity;
        void Delete<T>(T obj) where T : IEntity;
        void Update<T>(T obj) where T : IEntity;
        IEnumerable<T> ReadAll<T>() where T : IEntity;
        IEnumerable<T> ReadWithCondition<T>(string conditionName, object conditionValue) where T : IEntity;
    }
}
