using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Models.PublishedContent;

namespace bl.Models
{
    public partial class Subscription
    {
        public int? _AttemptsUsed;

        public string _SubscriptionTime;

        public int Id { get; private set; }

        public string ap_id { get; private set; }

        public int Attempts { get; private set; }

        public int AttemptsUsed
        {
            get
            {
                if (!_AttemptsUsed.HasValue)
                {
                    _AttemptsUsed = SharedPaypal.GetAttemptTrys(MemberId.ToString(), Id.ToString());
                }

                return _AttemptsUsed.Value;
            }
        }

        public string ItemName { get; private set; }

        public int MemberId { get; private set; }

        public string NodeName { get; private set; }

        public string path { get; private set; }

        public string PaymentStatus { get; private set; }

        public string SubStatus { get; private set; }

        public string SubscriptionTime
        {
            get
            {
                return _SubscriptionTime;
            }
            private set
            {
                _SubscriptionTime = value;
                if (!string.IsNullOrEmpty(_SubscriptionTime) && DateTime.TryParse(_SubscriptionTime, out var result))
                {
                    SubscriptionDateTime = result;
                }
            }
        }

        public DateTime SubscriptionDateTime { get; private set; }

        public string txn_id { get; private set; }

        public static Subscription FromSearchResult(SearchResult Source)
        {
            Subscription result = new Subscription();
            result.Id = Shared.TryParseInt(Source, "id");
            result.Attempts = Shared.TryParseInt(Source, SharedPaypal.Attempts);
            result.MemberId = Shared.TryParseInt(Source, "memberIdPurchased");
            if (result.MemberId == -1)
            {
                result.MemberId = Shared.TryParseInt(Source, "custom");
            }

            result.ItemName = Source.Fields[parachutewebdesign.com.clients.Will.SWTP.Paypal.Paypal.ItemName];
            result.NodeName = Source.Fields["nodeName"];
            result.path = Source.Fields["path"];
            result.PaymentStatus = Source.Fields[SharedPaypal.PaymentStatus];
            Shared.TrySetContainsKey(Source, SharedPaypal.SubStatus, delegate (string v)
            {
                result.SubStatus = v;
            });
            result.SubscriptionTime = Source.Fields[SharedPaypal.SubscriptionTime];
            Shared.TrySetContainsKey(Source, "txn_id", delegate (string v)
            {
                result.txn_id = v;
            });
            Shared.TrySetContainsKey(Source, "ap_id", delegate (string v)
            {
                result.ap_id = v;
            });
            return result;
        }
    }
}
