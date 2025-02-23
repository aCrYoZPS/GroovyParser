if (a){
    if (b){

    }
    else{
    }
}
else if (c){

}
else {
    d
}

class Person {
    def first
    def last

    // явно задаем сеттер
    void setFirst(first) {
        println "${this.first} is becoming ${first}"
        this.first = first
    }
}