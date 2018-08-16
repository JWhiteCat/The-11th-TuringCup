﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour, TListener
{
    public float moveSpeed;//角色移动的速度
    public float buffSpeed;
    public float buffTime;

    private bool isMoving; //角色目前是否在移动
    private Vector3 startPosition; //角色移动的初始位置
    private Vector3 targetPosition; //角色移动的目标位置

    private GameObject global; //全局脚本绑定的GameObject引用
    private MapManager map; //地图脚本引用

    private bool isBuffing;
    private float buffTiming;

    private Animator Anim;
    //玩家目前移动状态的只读接口
    public bool IsMoving()
    {
        return isMoving;
    }
    public bool IsBuffing()
    {
        return isBuffing;
    }
    private void Start()
    {
        Anim = GetComponent<Animator>();
        global = GameObject.FindGameObjectWithTag("Global");//此处Tag后期注意修改
        map = global.GetComponent<MapManager>(); //获取MapManager的引用

        isMoving = false;
        startPosition = new Vector3();
        targetPosition = new Vector3();

        isBuffing = false;
        buffTiming = 0f;

        //注册四个移动方向的监听器
        EventManager.Instance.AddListener(EVENT_TYPE.TURING_MOVE_NORTH, this);
        EventManager.Instance.AddListener(EVENT_TYPE.TURING_MOVE_SOUTH, this);
        EventManager.Instance.AddListener(EVENT_TYPE.TURING_MOVE_WEST, this);
        EventManager.Instance.AddListener(EVENT_TYPE.TURING_MOVE_EAST, this);
        EventManager.Instance.AddListener(EVENT_TYPE.SPEED_BUFF, this);
    }

    private void FixedUpdate()
    {
        //如果正在移动
        if (isMoving)
        {
            Move();
        }
        if (isBuffing)
        {
            BuffTiming();
        }
    }


    private void Move()
    {
        if (transform.position == targetPosition)
        {
            isMoving = false;
            Anim.SetBool("isMoving", false);
            return;
        }
        if (map.GetBoxType((int)targetPosition.x, (int)targetPosition.z) != 0) //判断目标位置是否可用
        {
            
            if(transform.position != startPosition)
            {
                targetPosition = startPosition;
                //TODO 如果初始位置也被占用的情况
            }
            else
            {
                isMoving = false;
                Anim.SetBool("isMoving", false);
                return;
            }
        }
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    private bool BuffSpeed()
    {
        moveSpeed += buffSpeed;
        isBuffing = true;
        return true;
    }

    private void BuffTiming()
    {
        buffTiming += Time.deltaTime;
        if(buffTiming >= buffTime)
        {
            moveSpeed -= buffSpeed;
            isBuffing = false;
        }
    }
    public bool OnEvent(EVENT_TYPE Event_Type, Component Sender, Object param, Dictionary<string, object> value)
    {
        switch (Event_Type)
        {
            case EVENT_TYPE.TURING_MOVE_NORTH:
                if (!isMoving && (int)(transform.position.z + 0.5) < 14)
                {
                    startPosition.Set((int)(transform.position.x + 0.5), 0f, (int)(transform.position.z + 0.5));
                    targetPosition.Set((int)(transform.position.x + 0.5), 0f, (int)(transform.position.z + 0.5) + 1);
                    isMoving = true;
                    Anim.SetBool("isMoving", true);
                    return true;
                }
                else
                {
                    return false;
                }
            case EVENT_TYPE.TURING_MOVE_SOUTH:
                if (!isMoving && (int)(transform.position.z + 0.5) > 0)
                {
                    startPosition.Set((int)(transform.position.x + 0.5), 0f, (int)(transform.position.z + 0.5));
                    targetPosition.Set((int)(transform.position.x + 0.5), 0f, (int)(transform.position.z + 0.5) - 1);
                    isMoving = true;
                    Anim.SetBool("isMoving", true);
                    return true;
                }
                else
                {
                    return false;
                }
            case EVENT_TYPE.TURING_MOVE_WEST:
                if (!isMoving && (int)(transform.position.x + 0.5) > 0)
                {
                    startPosition.Set((int)(transform.position.x + 0.5), 0f, (int)(transform.position.z + 0.5));
                    targetPosition.Set((int)(transform.position.x + 0.5) - 1, 0f, (int)(transform.position.z + 0.5));
                    isMoving = true;
                    Anim.SetBool("isMoving", true);
                    return true;
                }
                else
                {
                    return false;
                }
            case EVENT_TYPE.TURING_MOVE_EAST:
                if (!isMoving && (int)(transform.position.z + 0.5) < 14)
                {
                    startPosition.Set((int)(transform.position.x + 0.5), 0f, (int)(transform.position.z + 0.5));
                    targetPosition.Set((int)(transform.position.x + 0.5) + 1, 0f, (int)(transform.position.z + 0.5));
                    isMoving = true;
                    Anim.SetBool("isMoving", true);
                    return true;
                }
                else
                {
                    return false;
                }
            case EVENT_TYPE.SPEED_BUFF:
                BuffSpeed();
                return true;
            default: return false;
        }
    }


    public Object getGameObject()
    {
        return gameObject;
    }
}