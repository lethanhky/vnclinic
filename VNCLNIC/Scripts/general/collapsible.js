$(".collapsible_access").on("click", function () {
    this.classList.toggle("active");
    if (this.classList.contains("active")) {
        this.childNodes[0].classList.remove("fa-plus");
        this.childNodes[0].classList.add("fa-minus");
    } else {
        this.childNodes[0].classList.remove("fa-minus");
        this.childNodes[0].classList.add("fa-plus");
    }
    var con = "content_" + this.id;
    console.log('con', con);
    var content = document.getElementsByClassName(con);
    console.log(content);
    for (var k = 0; k < content.length; k++) {
        if (content[k].style.display === "table-row") {
            this
            content[k].style.display = "none";
        } else {
            content[k].style.display = "table-row";
        }
    }
});