using System;
using System.Threading.Tasks;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Infrastructure;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Model;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Service.Model
{
    public class OperatorService : BaseService
    {
        public OperatorService(Database db) : base(db)
        {
        }

        /// <summary>
        /// This action is not supportet in this Service.
        /// </summary>
        /// <param name="op"></param>
        /// <exception cref="NotSupportedException">This functions throws <see cref="NotSupportedException"/> since it is not supportet for this service.</exception>
#pragma warning disable IDE0060
        public void AddAsync(Operator op) => throw new NotSupportedException("This action is not supportet in this Service.");
#pragma warning restore IDE0060 

        public async Task UpdateAsync(Operator op)
        {
            // TODO check that op is correct.
            await base.UpdateAsync<Operator>(op);
        }

    }

}
