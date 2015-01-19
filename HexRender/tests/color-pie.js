﻿setViewPort(1600, 400, 90, 1);
setTitles({
    passingTestDate: "Дата прохождения теста",
    scorePerTest: "Количество баллов за тест"
});
setPieMode([
    { p: "Min", m: "#d9f", s: "#d9f" },
    { p: "Min", m: "#b3f", s: "#b3f" },
    { p: "Min", m: "#609", s: "#609" }
]);
setAdditionalsParam([
    { t: "Кол-во долгов за прошлый семестр", c: "#f00", w: 4, p: [1] },
    { t: "Уровень посещаемости занятий", c: "#66cc69", w: 4, p: [3, 1, 1, 1] }
]);

setThresholds([60, 75, 90, 100]);
setDates(["05.10.2013", "09.11.2013", "14.12.2013"]);
addStudent({
    n: "Смирнов В.В.",
    a: "18 лет",
    g: [[0, 0, 100], [0, 0, 0], [10, 10, 90]]
});
addStudent({
    n: "Иванов Л.Е.",
    a: "19 лет",
    g: [[50, 95, 95], [0, 0, 0], [90, 90, 90]]
});
addStudent({
    n: "Кузнецов С.В.",
    a: "20 лет",
    g: [[35, 70, 95], [0, 0, 0], [90, 80, 90]]
});
addDelimiter();
addStudent({
    n: "Морозов У.В.",
    a: "20 лет",
    g: [[50, 65, 80], [0, 0, 0], [90, 10, 80]]
});
addStudent({
    n: "Попов А.Д.",
    a: "18 лет",
    g: [[30, 55, 75], [0, 0, 0], [80, 65, 70]]
});
addStudent({
    n: "Новиков Г.А.",
    a: "22 года",
    g: [[30, 55, 75], [0, 0, 0], [80, 60, 65]]
});
addDelimiter();
addStudent({
    n: "Петров Я.И.",
    a: "25 лет",
    g: [[25, 50, 70], [50, 25, 10], [50, 50, 60]]
});
addStudent({
    n: "Попов С.А.",
    a: "20 лет",
    g: [[5, 10, 60], [80, 65, 10], [10, 10, 90]]
});