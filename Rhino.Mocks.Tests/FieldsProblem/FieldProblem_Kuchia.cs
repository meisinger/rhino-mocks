using System;
using Xunit;

namespace Rhino.Mocks.Tests.FieldsProblem
{
    public class FieldProblem_Kuchia : IDisposable
    {
        private IProblem _problem;
        private IDaoFactory _daoFactory;
        private IBLFactory _blFactory;

        [Fact]
        public void Method1_CallWithMocks_Returns10()
        {
            int result = Problem.Method1();
            
            Assert.Equal(10, result);
        }

        public IDaoFactory DaoFactoryMock
        {
            get
            {
                if (_daoFactory == null)
                    _daoFactory = MockRepository.Mock<IDaoFactory>();
                
                return _daoFactory;
            }
        }

        public IBLFactory BLFactoryMock
        {
            get
            {
                if (_blFactory == null)
                    _blFactory = MockRepository.Mock<IBLFactory>();
                
                return _blFactory;
            }
        }
        
        public IProblem Problem
        {
            get
            {
                if (_problem == null)
                    _problem = new Problem(BLFactoryMock, DaoFactoryMock);
                
                return _problem;
            }

        }

        public void Dispose()
        {
            _problem = null;
            _blFactory = null;
            _daoFactory = null;
        }
    }

    public interface IBLFactory
    {
    }

    public interface IDaoFactory
    {
    }

    public interface IProblem
    {
        int Method1();
    }

    public class Problem : BaseProblem, IProblem
    {
        public Problem(IBLFactory blFactory, IDaoFactory daoFactory)
            : base(blFactory, daoFactory)
        {

        }

        public int Method1()
        {
            return 10;
        }
    }

    public abstract class BaseProblem
    {
        private IBLFactory _blFactory;
        private IDaoFactory _daoFactory;

        public BaseProblem(IBLFactory blFactory, IDaoFactory daoFactory)
        {
            _blFactory = blFactory;
            _daoFactory = daoFactory;
        }
    }
}