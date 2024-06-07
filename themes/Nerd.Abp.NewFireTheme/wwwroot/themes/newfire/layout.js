$(document).ready(function () {

    $('a.nav_link').click(function () {
        $('.nav_link').removeClass('active');
        $(this).addClass('active');
        $(this).children(".link_toggle").toggleClass('rotate');
        
        let $subMenu = $(this).next(".submenu");
        $subMenu.toggleClass('show');
    });;

});

document.addEventListener("DOMContentLoaded", function (event) {

    const showNavbar = (toggleId, navId, bodyId, headerId) => {
        const toggle = document.getElementById(toggleId),
            nav = document.getElementById(navId),
            bodypd = document.getElementById(bodyId),
            headerpd = document.getElementById(headerId)

        // Validate that all variables exist
        if (toggle && nav && bodypd && headerpd) {
            toggle.addEventListener('click', () => {
                // show navbar
                nav.classList.toggle('nav-show')
                // add padding to body
                bodypd.classList.toggle('body-pd')
                // add padding to header
                headerpd.classList.toggle('body-pd')
            })
        }
    }

    showNavbar('header-toggle', 'nav-bar', 'body-pd', 'header')

    /*===== LINK ACTIVE =====*/
    $('.nav_link').click(function () {
        $('.nav_link').removeClass('active');
        $(this).addClass('active');
    });

    // Your code to run since DOM is loaded and ready
});

