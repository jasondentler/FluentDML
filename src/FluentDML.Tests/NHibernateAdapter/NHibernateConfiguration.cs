using NHibernate.Cfg;

namespace FluentDML.Tests.NHibernateAdapter
{
    public static class NHibernateConfiguration
    {

        public static Configuration Configuration { get; private set; }

        static NHibernateConfiguration()
        {
            Configuration = new Configuration().Configure();
        }


    }
}
