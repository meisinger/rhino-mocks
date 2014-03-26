using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Rhino.Mocks.Exceptions;

namespace Rhino.Mocks.Tests.FieldsProblem
{
    public class FieldProblem_Wendy
    {
        private ISearchPatternBuilder _searchPatternBuilder;
        private ImageFinder _imageFinder;

		public FieldProblem_Wendy()
        {
            _searchPatternBuilder = Repository.Mock<ISearchPatternBuilder>();
            _imageFinder = new ImageFinder(_searchPatternBuilder);
        }

        [Fact]
        public void SendingNullParamsValueShouldNotThrowNullReferenceException()
        {
            _searchPatternBuilder.Expect(x => x.CreateFromExtensions(ImageFinder.ImageExtensions))
                .Return(null);

            _imageFinder.FindImagePath();
        	Assert.Throws<ExpectationViolationException>(
        		() => Verify(_searchPatternBuilder));
        }

		[Fact]
		public void VerifyShouldFailIfDynamicMockWasCalledWithRepeatNever()
		{
            _searchPatternBuilder.Expect(x => x.CreateFromExtensions())
                .Repeat.Never();

			try
			{
				_searchPatternBuilder.CreateFromExtensions();
			}
			catch 
			{
				
			}

			Assert.Throws<ExpectationViolationException>(
				() => Verify(_searchPatternBuilder));
		}

        private void Verify(ISearchPatternBuilder builder)
        {
            builder.VerifyExpectations(true);
        }

        public string FindImagePath(string directoryToSearch)
        {
            _searchPatternBuilder.CreateFromExtensions(null);
            return null;
        }

        public interface ISearchPatternBuilder
        {
            string CreateFromExtensions(params string[] extensions);
        }

        public class ImageFinder
        {
            private readonly ISearchPatternBuilder builder;
            public static string[] ImageExtensions = { "png", "gif", "jpg", "bmp" };

            public ImageFinder(ISearchPatternBuilder builder)
            {
                this.builder = builder;
            }

            public void FindImagePath()
            {
                builder.CreateFromExtensions(null);
            }
        }
    }
}
