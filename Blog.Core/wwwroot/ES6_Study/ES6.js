import myHeader from '../components/header.vue';
import myFooter from '../components/footer.vue';

export default {
    components: { myHeader, myFooter },
    data() {
        return {
            id: this.$route.params.id,
            dat: {},
            isShow: true
        }
    },
    created() {
        this.getData()
    },
    methods: {
        getData() {
            var that = this;
            this.$api.get('Blog/Get/' + this.id, null, r => {
                r.data.bCreateTime = that.$utils.goodTime(r.data.bCreateTime)
                this.dat = r.data;
                this.isShow = false;
            })
        }
    },
    watch: {
        '$route'(to, from) {
            this.dat = {};
            this.isShow = true;
            this.id = to.params.id;
            this.getData();
        }
    }
}



var obj = new Object();
obj.Name = 'blog';
obj.address = 'beijing';

var obj = {
    name: 'blog',
    address: 'beijing',
    say: function () {
        alert("hello world");
    }
}

// 函数声明（定义和使用没有先后之分）
function obj() {
    alert('hi, 我是函数声明');
}

var obj = function () {
    // other things...
    alert('hi, 我是函数表达式方法');
}

// 构造函数
function Egperson(name, age) {
    this.name = name;
    this.age = age;
    this.sayName = function () {
        alert(this.name);
    }
}

var person = new Egperson('mike', '18');  // this --> person
person.sayName(); // 'mike'

// 普通函数
function egPerson(name, age) {
    this.name = name;
    this.age = age;
    this.sayName = function () {
        alert(this.name);
    }
}

egPerson('alice', '23');    // this --> window
window.sayName();           // 'alice'

// 1. 在命名规则上，构造函数一般是首字母大写，普通函数则是遵照小驼峰式命名法
// 2. 构造函数内部的this指向是新创建的person实例，而普通函数内部的this指向调用函数的对象
// （如果没有对象调用，默认为window）
// 3. 构造函数内部会创建一个实例，调用普通函数时则不会创建新的对象


/****************************************
 * 变量作用域
 * 
 ****************************************/
// var 
// var 定义的变量是函数级作用域，作用范围是在函数开始阶段和函数执行完成之前内都是存在的；
// 并且如果该函数内部还存在匿名函数等特殊函数,这个 var 出的变量在匿名函数中任然可以用 
//
function blog(bl) {
    if (bl) {
        var foo = "Blog";
    }
    console.log(foo);
}
blog(true); //=> Blog

// let
// let出的变量作用域是 块作用域，在离开某一代码块,该变量就会被销毁不存在
// 应当尽可能的避免用 var，用 let 来代替，除非你需要用到变量提升。
function Blog(bool) {
    if (bool) {
        let foo = "Blog";
    } else {
        console.log(foo);
    }
}
Blog(false); //这里会报错 Uncaught ReferenceError: foo is not defined

/****************************************
 * 常量 const
 * 
 * const 与 let 的基本用法相同，定义的变量都具有块级作用域，也不会发生变量提升。
 * 不同的地方在于，const 定义的变量，只能赋值一次。
 ****************************************/
const foo = 'Blog';
function Blog(bool) {
    if (bool) {
        foo = "Vue";
    } else {
        console.log(foo);
    }
}
Blog(true); //这里会报错 Identifier 'foo' has already been declared


/****************************************
 * 箭头函数
 * 
 * 在普通的click函数中 this 指向对象  $(".ok") ，因此，我们如果想要获取定义的对象中的数据（obj.data），那我们只能在 click 方法前，就去用一个 that 自定义变量来保存这个 this ，
 * 但是在箭头函数中就不一样了，this 始终指向定义函数时所在的对象（就是 obj 对象）；
 ****************************************/
var obj = {
    data: {
        book: '',
        price: 0,
        bookObj: null
    },
    bind() {
        var that = this;
        //普通函数
        $("._ok").click(function () {
            console.log(this);                        // 这个时候，this，就是 .ok 这个Html标签
            var bookItem = that.data.bookObj;         // 引用对象中数据（obj.data），需用变量that
            var _parice = $(bookItem).data("price");
            var _book = $(bookItem).data("book");
            that.data.book += _book + ",";
            that.data.price += parseInt(_parice);
        });

        //箭头函数
        $(".ok").click(() => {
            var bookItem = this.data.bookObj;//在箭头函数中，this指向的是定义函数时所在的对象
            var _parice = $(bookItem).data("price");
            var _book = $(bookItem).data("book");
            this.data.book += _book + ",";
            this.data.price += parseInt(_parice);
        });
    }
};

/****************************************
 * 参数默认值 && rest参数
 ****************************************/

// 定义参数默认值
function buyBook(price, count = 0.9) {
    return price * count;
}
buyBook(100);       // 90

// 甚至可以将方法的值赋给参数
function buyBook(price, count = GetCount()) {
    return price * count;
}
function GetCount() {
    return 100;
}

buyBook(200);       // 20000

/** 快速获取参数值 */
// ES6之前是这样的
function add(a, b, c) {
    let total = a + b + c;
    return total;
}
add(1, 2, 3);       // 6

// ES6你可以这么操作，提供了 rest 参数来访问多余变量
function sum(...num) {
    let total = 0;
    for (let i = 0; i < num.length; i++) {
        total = total + num[i];
    }
    return total;
}
sum(1, 2, 3, 4, 6); // 16

/****************************************
 * ES6中的表达式
 ****************************************/

/** 1、字符串表达式 */

// before
var name = 'id is ' + bid + ' ' + aid + '.'
var url = 'http://localhost:5000/api/values/' + id
// ES6
let id = 12, aid = 13, bid = 14;
var name = `id is ${aid} ${bid}`                        // id is 13 14
var url = `http://localhost:5000/api/values/${id}`      // http://localhost:5000/api/values/12

// 多行拼接
var roadPoem = '这个是一个段落'
    + '换了一行'
    + '增加了些内容'
    + 'dddddddddd'
// 但是在ES6中，可以使用反引号
var roadPoem2 = `这个是一个段落
   换了一行
    增加了些内容,
    dddddddddd
    结尾,`

