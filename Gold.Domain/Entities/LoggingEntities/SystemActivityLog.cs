using Gold.Domain.Entities.Base;
using Gold.SharedKernel.ExtentionMethods;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gold.Domain.Entities.LoggingEntities
{
    public class SystemActivityLog :IEntity<long>
    {
        public long Id { get ; set ; }
        /// <summary>
        /// get it from exception
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// get it from exception
        /// </summary>
        public string StackTrace { get; set; }
        /// <summary>
        /// get it from exception
        /// </summary>
        public string Source { get; set; }
        /// <summary>
        /// log date
        /// </summary>
        public DateTime RaiseDate { get; set; }
        /// <summary>
        /// extra data of log...
        /// </summary>
        public string ExtraData { get; set; }
    }
    
}
