using System;
using Xunit;
using Rhino.Mocks.Constraints;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	public class FieldProblem_leftend : IDisposable
	{
		private IAddAlbumPresenter viewMock;
		private IAlbum albumMock;
		
		public FieldProblem_leftend()
		{
            viewMock = Repository.Mock<IAddAlbumPresenter>();
            albumMock = Repository.Mock<IAlbum>();

            viewMock.ExpectEvent(x => x.Save += Arg<EventHandler<EventArgs>>.Is.NotNull);
		}

		public void Dispose()
		{
            viewMock.VerifyExpectations();
            albumMock.VerifyExpectations(true);
		}

		[Fact]
		public void VerifyAttachesToViewEvents()
		{
			new AddAlbumPresenter(viewMock);
		}

		[Fact]
		public void SaveEventShouldSetViewPropertiesCorrectly()
		{
            viewMock.Expect(x => x.AlbumToSave)
                .Return(albumMock);

            albumMock.Expect(x => x.Save());
            viewMock.Expect(x => x.ProcessSaveComplete());

			AddAlbumPresenter presenter = new AddAlbumPresenter(viewMock);
            viewMock.Raise(x => x.Save += null, EventArgs.Empty);
		}

		public interface IAlbum
		{
			string Name { get; set; }
			void Save();
		}

		public class Album : IAlbum
		{
			private string mName;

			public string Name
			{
				get { return mName; }
				set { mName = value; }
			}

			public Album()
			{
			}

			public void Save()
			{
				//code to save to db
			}
		}

		public interface IAddAlbumPresenter
		{
			IAlbum AlbumToSave { get; }
			event EventHandler<EventArgs> Save;
			void ProcessSaveComplete();
		}

		public class AddAlbumPresenter
		{
			private IAddAlbumPresenter mView;

			public AddAlbumPresenter(IAddAlbumPresenter view)
			{
				mView = view;
				Initialize();
			}

			private void Initialize()
			{
				mView.Save += new
					EventHandler<EventArgs>(mView_Save);
			}

			private void mView_Save(object sender, EventArgs e)
			{
				IAlbum newAlbum = mView.AlbumToSave;
				try
				{
					newAlbum.Save();
					mView.ProcessSaveComplete();
				}
				catch
				{
					//handle exception
				}
			}
		}
	}
}