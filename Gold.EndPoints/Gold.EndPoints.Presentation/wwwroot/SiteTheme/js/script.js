function handelChangeType (e) {
    // console.log(e.classList)
    var x = document.getElementById("password");
    console.log(x.type)
    if (x.type === "password") {
      x.type = "text";
      e.innerHTML = '<i class="fa fa-eye-slash"></i>';
    } else {
        x.type = "password";
      e.innerHTML = '<i class="fa fa-eye"></i>';
    }

}

const myDropdown = document.getElementById('navbarDropdown')
if(myDropdown)
{
    myDropdown.addEventListener('show.bs.dropdown', event => {
        let div = document.createElement("div");
        div.classList.add('backdrop')
        document.querySelector('body').append(div)
    })
    
    myDropdown.addEventListener('hide.bs.dropdown', event => {
        document.querySelector('.backdrop').remove();
    })
}


const Toast = Swal.mixin({
toast: true,
position: 'bottom-end',
showConfirmButton: false,
timer: 3000,
timerProgressBar: true,
didOpen: (toast) => {
    toast.addEventListener('mouseenter', Swal.stopTimer)
    toast.addEventListener('mouseleave', Swal.resumeTimer)
}
})

function login () {
    Toast.fire({
    icon: 'warning',
    title: 'رمز عبور وارد شده صحیح نمی باشد!'
    })
}

const fot_menu_action = document.querySelectorAll('.fot_menu_action')
if(fot_menu_action)
{
    fot_menu_action.forEach(element => {
        element.addEventListener('click' , function(){
         let menu = document.getElementById("footer_menu");
         if (menu.style.display === "none")
             menu.style.display = "block";
         else {
             menu.style.display = "none";
         }
     }) 
     });
}


const fot_user_menu_action = document.querySelectorAll('.fot_user_menu_action')
if(fot_user_menu_action)
{
    fot_user_menu_action.forEach(element => {
        element.addEventListener('click' , function(){
         let menu = document.getElementById("footer_user_menu");
         if (menu.style.display === "none")
             menu.style.display = "block";
         else {
             menu.style.display = "none";
         }
     }) 
     });
}
window.onhashchange = function(event) {
    event.preventDefault();
    console.log("sdfshodi ")
    document.getElementById("footer_user_menu").style.display = 'none'
    document.getElementById("footer_menu").style.display = 'none'
}


// window.addEventListener('popstate', function(event) {
//     console.log("teste1111")
//     event.preventDefault();
//     // document.getElementById("footer_user_menu").style.display = 'none'
//     // $('#footer_user_menu').modal('hide');
// });


// for test Loading Element
if(document.getElementById('loader'))
{
    setTimeout(()=> {
        document.getElementById('loader').style.display = 'none' ;
} , 500)
}


function submit () {
    Swal.fire({
    title: 'ثبت موفق ',
    text: "رسید شما با موفقیت ثبت شد",
    icon: 'success',
    confirmButtonColor: '#CC9D5A',
    confirmButtonText: 'تایید'
    })
}


function adjustFooterPosition() {
    var main = document.querySelector('main');
    var footer = document.querySelector('#mobile_footer');
    var loginArea = document.querySelector('#loginArea');
    // console.log("loginArea" , loginArea)
    var windowHeight = window.innerHeight;
    var documentHeight = document.documentElement.scrollHeight;
    var mainHeight = main?.offsetHeight;
    var footerHeight = footer?.offsetHeight;
  //   console.log("mainHeight"  ,mainHeight)
  //   console.log("windowHeight"  ,windowHeight)
  //   console.log("footerHeight"  ,footerHeight)
    // console.log("documentHeight - windowHeight"  ,documentHeight + " - " + windowHeight)
  //   console.log("windowHeight - mainHeight + footerHeight => "  ,windowHeight - (mainHeight + footerHeight))

  if(loginArea)
  {
    if(windowHeight < 500 )
    {
        loginArea?.classList.remove('login_content') ;
    }
    else {
        loginArea?.classList.add('login_content') ;
    }
  if (documentHeight > (windowHeight - 5)) {
    // console.log("windowHeight - mainHeight => "  ,windowHeight - mainHeight)
    if (windowHeight - mainHeight > 120) {
      footer.style.position = 'fixed';
      footer.style.marginTop = 'auto';
    } 
    else {
      footer.style.position = 'static';
    }
  } else {
    footer.style.position = 'fixed';
    footer.style.marginTop = 'auto';
  }
  }
     
  }

window.addEventListener('resize', adjustFooterPosition);
document.addEventListener('DOMContentLoaded', adjustFooterPosition);


function maxMinValidate(element , max , min , len , e=null ){
    let value = replacePersianDigits(element.val()) ;


    if(value.length > len)
    {
        value = value.slice(0, len);
        element.val(value);

    }
    else if(value.length < min)
    {
        // clearNumber(element);
        if(len ==2)
            element.val("");

    }
    else if(value > max)
    {
        if(max.toString().length === len)
            value = value.slice(0, len-1);
        else
        {
            value = value.slice(0, max.toString().length -1 );
        }
        element.val(value);
        // element.val(max)
    }
}

$(".number-input").on('input', function(){
    let value = $(this).val().replaceAll("," , "") ;
    if(!isNaN(value))
        value = parseInt($(this).val().replace(/\D/g,''),10);
    else
        value = "";
    $(this).val(value.toLocaleString());
});


$(document).on("input" , ".number " , function (e) {
    let element = $(this);
    let value = replacePersianDigits(element.val());
    let pattern = new RegExp(/^[۰-۹0-9/\s]+$/);
    // let pattern = new RegExp(/[+-]?([0-9]*[.])?[0-9]+/);
    // console.log(pattern.test(value))
    if (!pattern.test(value) && value != "")
    {
        // console.log("test");
        document.execCommand("undo");
        e.preventDefault();
        // return;
    }
})
$(document).on("input" , ".float " , function (e) {
    let element = $(this);
    let value = replacePersianDigits(element.val());
    let pattern = new RegExp(/^[۰-۹0-9./\s]+$/);
    if (!pattern.test(value) && value != "" || value.split(".").length > 2)
    {
        document.execCommand("undo");
        e.preventDefault();
    }
})

$(document).on("input" , ".percent" , function () {
    let element = $(this) ;
    maxMinValidate(element ,100 , 0 , 5)
})


$(document).on("input" , ".number-add-minus" , function () {
    let element = $(this) ;
    const min = element.data('min')  || null ;
    const max = element.data('min')  || null ;
    const len = element.data('len')  || null ;
    maxMinValidate(element ,max , min , len)
})

$(".btn-minus").click(function () {
    const next_el = $(this).next().next() ;
    let min = next_el.data("min")
    let value = replacePersianDigits(next_el.val());
    // console.log("value " , value)
    value = (( parseInt(value) || 0 ) - 1) < min ? min : ((parseInt(value) || 0) - 1) ;
    $(this).next().next().val(value)
})
$(".btn-plus").click(function () {
    const prev_el = $(this).prev() ;
    let max = prev_el.data("max")
    let value = replacePersianDigits(prev_el.val());
    value = (( parseInt(value) || 0 ) + 1) > max ? max : ((parseInt(value) || 0) + 1) ;
    $(this).prev().val(value)
})

function timer () {
    setInterval(() => {
        let date = new Date()
        date = date.toTimeString().split(" ")[0]
        date = date.split(":");
        if($("#timer-hour").text() !==  date[0])
            $("#timer-hour").text(date[0])
        if($("#timer-min").text() !==  date[1])
            $("#timer-min").text(date[1])

        $("#timer-second").text(date[2])
    },1000)
}

const replacePersianDigits = (value) => {
    return value
        .toString()
        .replace(/[\u06F0-\u06F9\u0660-\u0669]/g, (char) =>
            String.fromCharCode(char.charCodeAt(0) - 1728)
        );
}

$(".fa-share-nodes").click( async function () {
    const href = $(this).data("href") ;
    const title = $(this).data("title") ;
    navigator.clipboard.writeText(href).then(
        function() {
            /* clipboard successfully set */
            Toast.fire({
                icon: 'info',
                title: 'لینک مورد نظر کپی گردید'
            })
            // window.alert('Success! The text was copied to your clipboard')
        },
        function() {
            /* clipboard write failed */
            // window.alert('Opps! Your browser does not support the Clipboard API')
        })

    const shareData = {
        title: title,
        text: "",
        url: href,
    };

    try {
        await navigator.share(shareData);
        // resultPara.textContent = "MDN shared successfully";
    } catch (err) {
        // resultPara.textContent = `Error: ${err}`;
    }
})