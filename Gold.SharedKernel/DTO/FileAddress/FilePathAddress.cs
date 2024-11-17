using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gold.SharedKernel.DTO.FileAddress
{
    public class FilePathAddress
    {
        public string SocialNetworkIcon { get; set; }
        public string SocialNetworkIconUrl { get; set; }
        public string SellerProfileImage { get; set; }
        public string CollateralsDocs { get; set; }

        public string PaymentImages { get; set; }
        public string CustomerPaymentImages { get; set; }
        public string EditInformationRequest { get; set; }
        public string EditorImage { get; set; }
        public string EditorImageUrl { get; set; }
        public string CollateralsDocsImageBaseUrl { get; set; }

        public string CustomerProfileImage { get; set; }

        public string ArticleImages { get; set; }
        public string ArticleThumbnailImages { get; set; }
        public string ArticleVideos { get; set; }
        public int ArticleThumbnailImageWith { get; set; }
        public int ArticleThumbnailImageHeight { get; set; }
        public string ArticleDefaultImage { get; set; }


        public string ProductGallery { get; set; }
        public string ProductGalleryThumbnail { get; set; }
        public int ProductGalleryThumbnailWith { get; set; }
        public int ProductGalleryThumbnailHeight { get; set; }
        public string ProductDefaultImage{ get; set; }
    }
}
