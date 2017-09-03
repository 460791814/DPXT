var timer1;
function starttimer() {
    timer1= window.setTimeout(function () {
        window.location.href = "/PC/ChengGuo";
    }, 1000 * 60*10);  //timer1->1 当前是第一个定时器
}
starttimer();
function resettimer() {
    window.clearTimeout(timer1);
    starttimer();
}