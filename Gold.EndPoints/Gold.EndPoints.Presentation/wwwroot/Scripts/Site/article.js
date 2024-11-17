// active page
let page = 2;
// total page want to scroll
let scroll_page = 3;
// for final requeset
let final = false;
let searchTerm = '';
// for preventing a service to call when another request is running
let block = false;
$(document).ready(function () {
    $("#loading").hide()
    $("#pagiante").hide()
    //$("#listArea").html('');
    // get Element for first time
    handelRequest(page);
    setActivePaginate();
    openProductModal();
})

$(window).on("scroll", async function () {
    let windowHeight = $(window).height();
    let scrollTop = $(window).scrollTop();
    let divOffset = $("#listArea").offset().top;
    let divHeight = $("#listArea").height();

    if (page < scroll_page && !final && !block) {
        if (scrollTop + windowHeight >= divOffset + divHeight) {
            page = page + 1;
            await handelRequest(page)
        }
    }

});


$('.page-item').click(async function () {
    const el = $(this);
    final = true;
    block = true;
    let value = el.text().trim();
    page = value;

    setActivePaginate(el);

    $("html, body").animate({
        scrollTop: $("#listArea").offset().top - 100
    }, 300);
   // $("#listArea").html('');
    await handelRequest(page)
})

function setActivePaginate(el = null) {
    $('.page-item').each(function (element) {
        $(this).parent().removeClass('active');
        if (!el && $(this).text().trim() == scroll_page) {
            $(this).parent().addClass('active');
        }
    })
    if (el)
        el.parent().addClass('active');
}

async function handelRequest(pageNumber) {
    block = true;
    $("#loader").show();
    return new Promise(resolve => setTimeout(() => {
       // searchTerm = document.getElementById('searchTerm').value;
        let url = `/Article/All?s=${searchTerm}&page=${pageNumber}`;

        $.getJSON(url, function (items) {
            renderItem(items);
            block = false;
            $("#loader").hide()
            resolve(items);
        })
    }, 20))
}

//attach proucts to last items
function renderItem(data) {
    let html = ''
   
    //let html = '<div class="col-12  mb-3 text-light text-center"><h3>آیتمی جهت نمایش وجود ندارد</h3></div>';
    //if (page >= scroll_page) {
    //    $("#pagiante").show();
    //    block = true;
    //}
    if (page == 1) {
        document.getElementById('listArea').innerHTML = html;
    }
    if (data.length<=0) {
        //html = `<div class="col-12  mb-3 text-light text-center">
        //                                <h3>آیتمی جهت نمایش وجود ندارد</h3>
        //                            </div>`;
        document.getElementById('listArea').innerHTML = html;
    } else {
        data.forEach(el => {
            //let imageName = '';

            //console.log(`ImageFileName: ${el.ImageFileName}`)

            //if (el.ImageFileName == null) {
            //    imageName ='PDefault.png'
            //} else {
                
            //    if (el.ImageFileName.length > 0) {
            //        imageName = el.ImageFileName;
            //    } else {
            //        imageName = 'PDefault.png';
            //    }
            //}
            console.log(`asascasca ${el.ImageFileName}`)
           
            html = `<div class="col-12 col-md-6 mb-3">
                                        <div class="card learn-card-area" onclick="getPostInfo('${el.Id}')">
                                            <div class="card-body">
                                                <div class="row">
                                                    <div class="col-4 ${el.HasVideo ? 'learn-img-media' : ''}">
                                                         <img src="/Article/ThumbnailImage/${el.ImageFileName}" alt="${el.Title}">
                                                    </div>
                                                    <div class="col-8 learn-title">
                                                        <a href="#">
                                                                    ${el.Title}
                                                        </a>
                                                        <button class="btn btn-primary mb-3 mb-md-0 small rounded-pill">
                                                            بیش‌تر بخوانید ...
                                                            <b>
                                                            <i class="fa-regular fa-sparkles me-1"></i>
                                                            </b>
                                                        </button>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>`;
            $("#listArea").append(html);
            
        })
    }
}

$("#btn-rules").click(function () {
    $("#buy-rule-area").hide();
    $("#buy-factor-area").show();
});

function unsetPostInfoData() {
    document.getElementById('postShareButton').setAttribute('data-href', '');
    document.getElementById('postShareButton').setAttribute('data-title', '');
    document.getElementById('postTitle').innerText = '';
    document.getElementById('postDescription').innerHTML = '';
    document.getElementById('imageOrVideo').innerHTML = '';
}
$(document).ready(function () {
    $(function () {
        $('#blogContentInfo').modal({
            show: false
        }).on('hidden.bs.modal', function () {
            //var myVideo = document.querySelector("#blogContentInfo ");
            document.getElementById("imageOrVideo").innerHTML = '';
            //let vid = document.getElementById("postVideo");
            //vid.pause();
        });
    });
});
function getPostInfo(postId) {
    let url = `/Article/Detail/${postId}`;
    $.getJSON(url, function (result) {
        if (result.IsSuccess) {
            unsetPostInfoData();
            let shareUrl = `${window.location.origin}/Article?p=${postId}`
            document.getElementById('postShareButton').setAttribute('data-href', shareUrl);
            document.getElementById('postShareButton').setAttribute('data-title', `${result.Data.Title}`);
            document.getElementById('postTitle').innerText = result.Data.Title;
            document.getElementById('postDescription').innerHTML = result.Data.Description;
            let videoOrImageHtml = '';
            if (result.Data.HasVideo) {
                if (result.Data.VideoFileName!=null) {
                    if (result.Data.VideoFileName.length > 0) {
                        //videoOrImageHtml = `<embed type="video/mp4" src="/Article/Video/${result.Data.VideoFileName}" class="mb-4">`;
                        videoOrImageHtml = `<div class='video mb-4 rounded'>
                                           <video id='postVideo' width="640" height="480" controls>
                                                <source id="fileSrc" src="/Article/Video/${result.Data.VideoFileName}" style='width:inherit' type="video/mp4">
                                          </video>
                                        </div>`;
                    }
                }
            } else {
                if (result.Data.ImageFileName != null) {
                    if (result.Data.ImageFileName.length > 0) {
                        //videoOrImageHtml = `<embed type="image/jpg" src="/Article/Image/${result.Data.ImageFileName}" class="mb-4">`;
                        videoOrImageHtml = ` <div class='video mb-4 rounded'>
                                               <img src="/Article/Image/${result.Data.ImageFileName}"  class="rounded" style='width:inherit'>
                                        </div>`;
                    }
                }
            }
            document.getElementById('imageOrVideo').innerHTML = videoOrImageHtml;
            $('#blogContentInfo').modal('show');
        } else {
            ShowAlertToast(result.Type, result.Title, result.Message);
        }
    });
}

function openProductModal() {
    try {
        var items = new URLSearchParams(window.location.search);
        let postId = items.get('p');
        if (postId.length > 0) {
            getPostInfo(postId);
        }
    } catch (e) {

    }
}


function search(ele) {
    if (event.key === 'Enter') {
        //alert(ele.value);
        page = 1;
        searchTerm = ele.value;
       handelRequest(1)
    }
}