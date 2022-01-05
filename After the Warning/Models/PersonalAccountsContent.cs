﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;

namespace Models
{
    public class PersonalAccountsContent
    {
        public string imgUrl { get; set; }
        public List<IPublishedContent> lstArticles { get; set; }

    }
}