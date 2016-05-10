using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WareDealer.Mode
{
    /// <summary>
    /// 评价信息模型
    /// </summary>
    public class BadpostMsg
    {
        public Int64 id { get; set; }
        public string guid { get; set; }
        public string content { get; set; }
        public string creationTime { get; set; }
        public bool isTop { get; set; }
        public string referenceId { get; set; }
        public string referenceImage { get; set; }
        public string referenceName { get; set; }
        public string referenceTime { get; set; }
        public string referenceType { get; set; }
        public int referenceTypeId { get; set; }
        public int firstCategory { get; set; }
        public int secondCategory { get; set; }
        public int thirdCategory { get; set; }
        public int replyCount { get; set; }
        public int score { get; set; }
        public int status { get; set; }
        public string title { get; set; }
        public int usefulVoteCount { get; set; }
        public int uselessVoteCount { get; set; }
        public string userImage { get; set; }
        public string userImageUrl { get; set; }
        public string userLevelId { get; set; }
        public string userProvince { get; set; }
        public string userRegisterTime { get; set; }
        public int viewCount { get; set; }
        public int orderId { get; set; }
        public bool isReplyGrade { get; set; }
        public string nickname { get; set; }
        public int userClient { get; set; }
        public string productColor { get; set; }
        public string productSize { get; set; }
        public int integral { get; set; }
        public int anonymousFlag { get; set; }
        public string userLevelName { get; set; }
        public bool recommend { get; set; }
        public string userLevelColor { get; set; }
        public string userClientShow { get; set; }
        public bool isMobile { get; set; }
        public int days { get; set; }
    }

    public class MsgSummary
    {
        public int beginRowNumber { get; set; }
        public int endRowNumber { get; set; }
        public Int64 skuId { get; set; }
        public Int64 productId { get; set; }
        public int score1Count { get; set; }
        public int score2Count { get; set; }
        public int score3Count { get; set; }
        public int score4Count { get; set; }
        public int score5Count { get; set; }
        public int showCount { get; set; }
        public int commentCount { get; set; }
        public int averageScore { get; set; }
        public int goodCount { get; set; }
        public double goodRate { get; set; }
        public int goodRateShow { get; set; }
        public int goodRateStyle { get; set; }
        public int generalCount { get; set; }
        public double generalRate { get; set; }
        public int generalRateShow { get; set; }
        public int generalRateStyle { get; set; }
        public int poorCount { get; set; }
        public double poorRate { get; set; }
        public int poorRateShow { get; set; }
        public int poorRateStyle { get; set; }
    }

    /// <summary>
    /// 评价数据模型
    /// </summary>
    public class EvaluateMsg
    {
        /// <summary>
        /// 产品属性
        /// </summary>
        public object productAttr { get; set; }
        /// <summary>
        /// 评价属性
        /// </summary>
        public MsgSummary productCommentSummary { get; set; }
        /// <summary>
        /// 评价标签
        /// </summary>
        public List<HotCommentTag> hotCommentTagStatistics { get; set; }
        public string jwotestProduct { get; set; }
        public int score { get; set; }
        public int soType { get; set; }
        public int imageListCount { get; set; }

        private List<BadpostMsg> _msgs = new List<BadpostMsg>();
        /// <summary>
        /// 评价信息
        /// </summary>
        public List<BadpostMsg> comments
        {
            get { return _msgs; }
            set { _msgs = value; }
        }
        public object topFiveCommentVos { get; set; }

    }
    /// <summary>
    /// 买家印象
    /// </summary>
    public class HotCommentTag
    {
        public int id { get; set; }
        public string name { get; set; }
        public int status { get; set; }
        public int rid { get; set; }
        public int productId { get; set; }
        public int count { get; set; }
        public string created { get; set; }
        public string modified { get; set; }
    }
}
