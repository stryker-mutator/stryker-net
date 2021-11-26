using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms.Models;
using Xamarin.Forms.Services;
using Xunit;

namespace Xamarin.Forms.Tests
{
    public class UnitTest1
    {
        [Fact]
        public async Task Test1()
        {
            var x = new MockDataStore();
            await x.AddItemAsync(new Item());
            var count = (await x.GetItemsAsync()).Count();
            Assert.True(count == 7);
        }
    }
}
