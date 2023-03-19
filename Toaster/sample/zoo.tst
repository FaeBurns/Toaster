stl .p2:8 $acc; set ports 2-7 to the low bits of acc
sth .p10..p17 $acc; set ports 10-17 to the high bits of acc

mov $reg1 0x0000

:reset

; loop load from ports until incoming value is 0x0000
:loop
; load 16 bits from same ports into reg0
ldl $reg0 .p2:8
ldh $reg0 .p10..p17

bne loop $reg0 $reg1

; sph - set pin(s) high - sph .p0
; spl - set pin(s) low - spl .p0..p3
; stp - set pin(s) value - stp .p0:2 1

sph .p0
slp 2
spl .p0

jrs reset ; jump reset stack