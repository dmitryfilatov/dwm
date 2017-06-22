using DeltaWorkMonitoring.Infrastructure;
using DeltaWorkMonitoring.Models;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DeltaWorkMonitoring.Tests
{
    public class TaskRepositoryTests
    {
        [Theory, MemberData(nameof(TaskData))]
        public void GetTasks_VariousPeriod_ReturnsCorrectTask(string taskName, DateTime dt, TaskPeriod period)
        {
            var userId = Guid.NewGuid();
            var data = new List<WorkTask>
            {
                new WorkTask { Name = taskName, UserId = userId, Created = dt },
            };

            var set = FakeDbSet(data);
            var ctx = Substitute.For<ApplicationDbContext>(new DbContextOptions<ApplicationDbContext>());
            ctx.GetTasks().Returns(set);

            var repo = new TaskRepository(ctx);
            var tasks = repo.GetTasks(userId, period).ToArray();
            Assert.Equal(tasks.Length, 1);
            Assert.Equal(tasks[0].Name, taskName);
        }

        public static IEnumerable<object[]> TaskData
        {
            get
            {
                return new[]
                {
                    new object[] { "Task", DateTime.Now.GetRandomDateTime(TaskPeriod.Day), TaskPeriod.Day },
                    new object[] { "Task", DateTime.Now.GetRandomDateTime(TaskPeriod.Week), TaskPeriod.Week },
                    new object[] { "Task", DateTime.Now.GetRandomDateTime(TaskPeriod.Month), TaskPeriod.Month },
                    new object[] { "Task", DateTime.Now.GetRandomDateTime(TaskPeriod.Quarter), TaskPeriod.Quarter },
                    new object[] { "Task", DateTime.Now.GetRandomDateTime(TaskPeriod.Year), TaskPeriod.Year }
                };
            }
        }

        public static DbSet<T> FakeDbSet<T>(List<T> data) where T : class
        {
            var _data = data.AsQueryable();
            var fakeDbSet = Substitute.For<DbSet<T>, IQueryable<T>>();
            ((IQueryable<T>)fakeDbSet).Provider.Returns(_data.Provider);
            ((IQueryable<T>)fakeDbSet).Expression.Returns(_data.Expression);
            ((IQueryable<T>)fakeDbSet).ElementType.Returns(_data.ElementType);
            ((IQueryable<T>)fakeDbSet).GetEnumerator().Returns(_data.GetEnumerator());
            return fakeDbSet;
        }
    }
}
